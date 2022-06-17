using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tracker.Database;

namespace tracker.Services;

public class TrackingService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TrackingService> _logger;

    public TrackingService(IServiceProvider provider, ILogger<TrackingService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        new Timer((o) => TrackProcesses(token), null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1));
        await Task.Delay(1, token);
    }

    public async void TrackProcesses(CancellationToken token)
    {
        try
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var currentProcesses = Process.GetProcesses().ToList();
            var dbUpdated = false;

            foreach (var process in currentProcesses)
            {
                string? path;
                try { path = process.MainModule?.FileName; }
                catch (Exception ex) // Suppress exceptions for processes that have protected main modules
                {
                    if (ex is Win32Exception or InvalidOperationException)
                        continue;
                    throw;
                }

                if (path is null)
                    continue;
                if(path.Contains("notepad"))
                    Console.WriteLine("b");
                path = path.Replace(@"\", @"/");
                if (!context.Processes.Where(x => x.Path != null).Any(x => x.Path!.ToLower() == path.ToLower()))
                    continue;

                var trackedProcess = context.Processes.AsQueryable().OrderBy(x => x.Id).Where(x => x.Path != null).FirstOrDefault(x => x.Path!.ToLower() == path.ToLower());
                if (trackedProcess is null)
                    continue;

                trackedProcess.LastAccessed = DateTime.UtcNow.ToString("o");
                trackedProcess.MinutesRan += 1;
                _logger.LogInformation("Added minute to {name}", trackedProcess.Name ?? "NA");

                if (!dbUpdated)
                    dbUpdated = true;
            }

            if (dbUpdated)
                await context.SaveChangesAsync(token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

    }
}
