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
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));
    private readonly List<string> _trackedProcesses = new();
    private readonly string[] _blockedProcessNames = { "svchost", "Idle", "System" };

    public TrackingService(IServiceProvider provider, ILogger<TrackingService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(token) && !token.IsCancellationRequested)
            {
                await TrackProcesses(token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Swallow
        }
    }

    public async Task TrackProcesses(CancellationToken token)
    {
        try
        {
            var context = _provider.GetRequiredService<DataContext>();
            var currentProcesses = Process.GetProcesses().Where(x => !_blockedProcessNames.Contains(x.ProcessName)).ToList();
            var dbUpdated = false;

            foreach (var process in currentProcesses)
            {

                var procName = process.ProcessName;
                string? path = null;
                try { path = process.MainModule?.FileName; }
                catch (Exception ex) // Suppress exceptions for processes that have protected main modules
                {
                    if (ex is not Win32Exception and not InvalidOperationException)
                        throw;
                }

                path = path?.Replace(@"\", @"/");
                if (_trackedProcesses.Any(x => x == path?.ToLower() || x == procName))
                    continue;

                var trackedProcess = context.Processes.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == procName.ToLower());
                if (trackedProcess is null)
                    if (path is not null)
                        trackedProcess = context.Processes.FirstOrDefault(x => x.Path != null && x.Path.ToLower() == path.ToLower());
                    else
                        continue;

                if (trackedProcess is null || !trackedProcess.Tracking)
                    continue;

                trackedProcess.LastAccessed = DateTimeOffset.UtcNow.ToString("o");
                trackedProcess.MinutesRan += 1;
                _trackedProcesses.Add(trackedProcess.Path!.ToLower());

                if (!dbUpdated)
                    dbUpdated = true;
            }

            if (dbUpdated)
                await context.SaveChangesAsync(token);
            _trackedProcesses.Clear();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

    }
}
