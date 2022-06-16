using System.Net.Http.Json;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using tracker.Database;

namespace tracker.Services;

public class GameDetectionService
{
    private readonly DataContext _context;
    private readonly ILogger<GameDetectionService> _logger;
    private const string RegPath = @"Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam";

    public GameDetectionService(DataContext context, ILogger<GameDetectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task StartAutomaticDetectionAsync()
    {
        await GetSteamGamesAsync();
    }

    private async Task GetSteamGamesAsync()
    {
        await using var context = _context;
        string? steamPath = null;
        if (context.Whitelists.Any(x => x.PathName == "Steam"))
        {
            var setting = await context.Whitelists.FirstOrDefaultAsync(x => x.PathName == "Steam");
            if (setting is null)
            {
                _logger.LogWarning("Cannot find Steam in whitelist unexpectedly. Cannot find Steam games automatically.");
                return;
            }

            if (string.IsNullOrEmpty(setting.PathName))
            {
                _logger.LogWarning("Steam found in the whitelist, but no path is set. Cannot find Steam games automatically.");
                return;
            }

            steamPath = setting.PathName;
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

        using var http = new HttpClient();
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

