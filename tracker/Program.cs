using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using tracker.Database;
using tracker.Services;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, logging) =>
    {
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddConsole();
        logging.AddFilter("Microsoft.EntityFrameworkCore.*", LogLevel.Warning);
        logging.AddFilter("Microsoft.Hosting.*", LogLevel.Warning);
    })
    .ConfigureServices((context, services) =>
    {
        var cString = context.Configuration.GetConnectionString("Sqlite");
        services
            .AddSqlite<DataContext>(cString)
            .AddSingleton(new HttpClient())
            .AddScoped<DbInitService>()
            .AddScoped<GameDetectionService>()
            .AddHostedService<ConsoleService>();
    })
    .Build();

await host.Services.GetRequiredService<DbInitService>().InitializeDatabaseAsync();
await host.Services.GetRequiredService<GameDetectionService>().StartAutomaticDetectionAsync();
await Task.Factory.StartNew(async () => await new TrackingService(CancellationToken.None).TrackProcesses(), TaskCreationOptions.LongRunning); 
await host.RunAsync();