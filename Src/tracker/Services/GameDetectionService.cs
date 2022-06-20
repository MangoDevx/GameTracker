using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using tracker.Database;
using tracker.Database.DbModels;

namespace tracker.Services;

public class GameDetectionService
{
    private readonly DataContext _context;
    private readonly ILogger<GameDetectionService> _logger;
    private const string RegPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";
    private readonly HttpClient _http;

    public GameDetectionService(DataContext context, ILogger<GameDetectionService> logger, HttpClient http)
    {
        _context = context;
        _logger = logger;
        _http = http;
    }

    public async Task StartAutomaticDetectionAsync()
    {
        await GetSteamGamesAsync();
        _logger.LogInformation(">> Automatic Steam detection completed.");
        // TODO: Epic game detection
    }

    private async Task GetSteamGamesAsync()
    {
        var context = _context;
        string? steamPath;
        if (context.Processes.Any(x => x.Name == "Steam"))
        {
            var setting = await context.Processes.FirstOrDefaultAsync(x => x.Name == "Steam");
            if (setting is null)
            {
                _logger.LogWarning(">> Cannot find Steam in tracked processes unexpectedly. Cannot find Steam games automatically.");
                return;
            }

            if (string.IsNullOrEmpty(setting.Path))
            {
                _logger.LogWarning(">> Steam found in the tracked processes, but no path is set. Cannot find Steam games automatically.");
                return;
            }

            steamPath = setting.Path;
        }
        else
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isWindows)
            {
                _logger.LogWarning(">> Cannot automatically detect Steam install path on a non-Windows machine. Please use the whitelist feature.");
                return;
            }

            steamPath = (string?)Registry.GetValue(RegPath, "InstallPath", null);
            if (steamPath is null)
            {
                _logger.LogInformation(">> No Steam install path detected. Skipping Steam detection");
                return;
            }
        }

        if (steamPath is null)
        {
            _logger.LogWarning(">> Failed to find Steam's install path. Cannot automatically detect Steam games.");
            return;
        }

        var libraryPaths = await VdfReader.VdfReader.ReadVdfLibraryPathsAsync(steamPath + "/steamapps/libraryfolders.vdf");
        var gameDirectories = new List<string>();

        foreach (var path in libraryPaths)
        {
            if (Directory.Exists(path + "/steamapps"))
                gameDirectories.Add(path + "/steamapps/common");
            else if (Directory.Exists(path + "/common"))
                gameDirectories.Add(path + "/common");
            else
                gameDirectories.Add(path);
        }

        var gameModels = new List<TrackedProcess>();
        foreach (var dir in gameDirectories)
        {
            var subDirectories = Directory.GetDirectories(dir);
            foreach (var subDir in subDirectories)
            {
                var filePaths = Directory.GetFiles(subDir).ToList();

                // Solves the issue where some games hide the files behind a singular folder
                if (!filePaths.Any() && Directory.GetDirectories(subDir).Length == 1)
                    filePaths = Directory.GetFiles(Directory.GetDirectories(subDir)[0]).ToList();

                filePaths = filePaths.Where(x => x.Split('.').Last() == "exe" && !x.Contains("Unity")).ToList();

                if (!filePaths.Any())
                {
                    _logger.LogInformation(">> Could not find any .exes for {subDir}", subDir);
                    continue;
                }

                var biggestFile = ("", 0L);
                foreach (var filePath in filePaths)
                {
                    var file = new FileInfo(filePath);
                    var size = file.Length;
                    if (size > biggestFile.Item2)
                        biggestFile = (filePath, size);
                }

                biggestFile.Item1 = biggestFile.Item1.Replace(@"\", "/");
                if (string.IsNullOrEmpty(biggestFile.Item1) || _context.Blacklists.Any(x => x.Path == biggestFile.Item1))
                    continue;

                var gameName = biggestFile.Item1.Split('/').Last().Split('.').First();
                if (string.IsNullOrEmpty(gameName))
                    gameName = biggestFile.Item1;

                gameModels.Add(new TrackedProcess { Name = gameName, DisplayName = gameName, Path = biggestFile.Item1, MinutesRan = 0, LastAccessed = DateTime.UtcNow.ToString("o"), Tracking = true });
            }

            var didDbUpdate = false;
            foreach (var game in gameModels.Where(game => !context.Processes.Any(x => x.Path == game.Path)))
            {
                context.Processes.Add(game);
                didDbUpdate = true;
                _logger.LogInformation(">> Added executable {exe} to the process list from Steam!", game.Name);
            }

            if (didDbUpdate)
                await context.SaveChangesAsync();
        }
    }
}

