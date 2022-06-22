using System.Diagnostics;
using System.Text.Json;
using System.Timers;
using Microsoft.Data.Sqlite;
using Timer = System.Timers.Timer;

namespace TrackerApi;

class Program
{

    private static Timer _checkTimer = new(TimeSpan.FromHours(1).TotalMilliseconds);

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls();
        builder.WebHost.UseKestrel();
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(x =>
        {
            x.AddDefaultPolicy(y =>
            {
                y.WithOrigins("http://localhost:3000", "https://localhost:3000");
            });
        });
        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseHttpsRedirection();

        var data = new List<TrackerData>();
        app.MapGet("/trackerapi", () =>
        {
            var connection = new SqliteConnection(app.Configuration.GetConnectionString("Sqlite"));
            var sqlCommand = new SqliteCommand("SELECT DisplayName, MinutesRan FROM Processes WHERE (Tracking > 0) AND (MinutesRan > 0) ORDER BY DisplayName ASC;", connection);
            connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var name = reader["DisplayName"] as string;
                var minRanString = reader["MinutesRan"].ToString();
                if (!double.TryParse(minRanString, out var minRan) || name is null)
                    continue;

                data.Add(new TrackerData(name, minRan));
            }

            connection.Close();
            var json = JsonSerializer.Serialize(data);
            data.Clear();
            return json;
        }).WithName("GetTrackedData");

        _checkTimer.Elapsed += CheckProcesses;
        _checkTimer.Start();
        app.Run();
    }

    private static void CheckProcesses(object? sender, ElapsedEventArgs e)
    {
        var processes = Process.GetProcesses();
        if (processes.Any(x => x.ProcessName.ToLower() == "tracker"))
            return;

        Console.WriteLine("Tracker not found, exiting!");
        Environment.Exit(-1);
    }
}

internal record TrackerData(string Name, double MinutesRan);