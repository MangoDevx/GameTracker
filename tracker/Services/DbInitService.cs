using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tracker.Database;
using tracker.Database.DbModels;

namespace tracker.Services;

public class DbInitService
{
    private readonly ILogger<DbInitService> _logger;
    private readonly DataContext _dataContext;

    public DbInitService(ILogger<DbInitService> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            var context = _dataContext;
            await context.Database.MigrateAsync();

            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isWindows && !context.Processes.Any(x => x.Name == "Steam"))
            {
                await context.Processes.AddAsync(new TrackedProcess {Name = "Steam", Path = null, Tracking = false});
                _logger.LogWarning("Cannot automatically determine steam path. Please set one if you want to automatically detect Steam games.");
            }
            await context.SaveChangesAsync();
            _logger.LogInformation("Database initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}

