using Microsoft.EntityFrameworkCore;
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
    })
    .ConfigureServices((context, services) =>
    {
        var cString = context.Configuration.GetConnectionString("Sqlite");
        services
            .AddDbContext<DataContext>(builder => builder.UseSqlite(cString))
            .AddSingleton(new HttpClient())
            .AddScoped<DbInitService>()
            .AddScoped<GameDetectionService>();
    })
    .Build();

await host.Services.GetRequiredService<DbInitService>().InitializeDatabaseAsync();
await host.Services.GetRequiredService<GameDetectionService>().StartAutomaticDetectionAsync();

await host.RunAsync();