using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using tracker.Database;
using tracker.Database.DbModels;
using tracker.Models;

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
        _logger.LogInformation("Automatic Steam detection completed.");
    }

    private async Task GetSteamGamesAsync()
    {
        var context = _context;
        string? steamPath = null;
        if (context.Processes.Any(x => x.Name == "Steam"))
        {
            var setting = await context.Processes.FirstOrDefaultAsync(x => x.Name == "Steam");
            if (setting is null)
            {
                _logger.LogWarning("Cannot find Steam in tracked processes unexpectedly. Cannot find Steam games automatically.");
                return;
            }

            if (string.IsNullOrEmpty(setting.Path))
            {
                _logger.LogWarning("Steam found in the tracked processes, but no path is set. Cannot find Steam games automatically.");
                return;
            }

            steamPath = setting.Path;
        }
        else
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isWindows)
            {
                _logger.LogWarning("Cannot automatically detect Steam install path on a non-Windows machine. Please use the whitelist feature.");
                return;
            }

            steamPath = (string?)Registry.GetValue(RegPath, "InstallPath", null);
            if (steamPath is null)
            {
                _logger.LogInformation("No Steam install path detected. Skipping Steam detection");
                return;
            }
        }

        if (steamPath is null)
        {
            _logger.LogWarning("Failed to find Steam's install path. Cannot automatically detect Steam games.");
            return;
        }

        await CheckSteamDataAsync();
        // TODO: Add support for multiple install paths provided in the .vdf (low prio)

        var vdfPath = steamPath + "/steamapps/libraryfolders.vdf";
        if (!File.Exists(vdfPath))
        {
            _logger.LogWarning("Vdf path did not exist. Cannot automatically detect Steam games.");
            return;
        }

        var vdfData = (await VdfReader.VdfReader.ReadVdfDataAsync(steamPath + "/steamapps/libraryfolders.vdf")).ToList();
        var gameIds = new HashSet<long>();
        foreach (var key in vdfData.Where(vdf => vdf is not null).SelectMany(vdf => vdf!.apps.Keys))
        {
            if (long.TryParse(key, out var gameId))
                gameIds.Add(gameId);
        }

        var catalogueJson = await File.ReadAllTextAsync("SteamData.json");
        var steamCatalogueData = JsonSerializer.Deserialize<SteamCatalogue>(catalogueJson);

        if (!vdfData.Any() || steamCatalogueData?.applist is null)
        {
            _logger.LogWarning("Failed to load the proper vdfdata or steam catalogue data. Cannot automatically detect games");
            return;
        }

        var dbUpdated = false;
        foreach (var gameId in gameIds)
        {
            var steamGame = steamCatalogueData.applist?.apps.FirstOrDefault(x => x?.appid == gameId);
            if (steamGame is null)
            {
                _logger.LogWarning("Failed to find {gameId} in steam catalogue.", gameId);
                continue;
            }

            if (_context.Processes.Any(x => x.Name == steamGame.name + ".exe"))
                continue;

            await _context.AddAsync(new TrackedProcess { Name = steamGame.name + ".exe", Path = string.Empty, LastAccessed = DateTime.UtcNow.ToString("o")});
            _logger.LogInformation("Added game {gameName} to the tracking list!", steamGame.name);

            if (!dbUpdated)
                dbUpdated = true;
        }

        if (dbUpdated)
            await _context.SaveChangesAsync();
    }

    private async Task CheckSteamDataAsync()
    {
        if (File.Exists("SteamData.json"))
        {
            var lastWriteTime = File.GetLastWriteTimeUtc("SteamData.json");
            if ((DateTime.UtcNow - lastWriteTime).TotalDays < 30)
                return;
        }
        else
            File.Create("SteamData.json");

        using var http = _http;
        var response = await http.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v2?format=json");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to update Steam catalogue.");
            return;
        }

        var newSteamData = await response.Content.ReadAsStringAsync();
        await File.WriteAllTextAsync("SteamData.json", newSteamData);

        _logger.LogInformation("Steam catalogue updated");
    }
}

