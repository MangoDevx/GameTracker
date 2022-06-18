using System.Text.Json;
using Microsoft.Data.Sqlite;

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
    var sqlCommand = new SqliteCommand("SELECT Name, MinutesRan FROM Processes WHERE (Tracking > 0) AND (MinutesRan > 0) ORDER BY Name ASC;", connection);
    connection.Open();
    var reader = sqlCommand.ExecuteReader();
    while (reader.Read())
    {
        var name = reader["Name"] as string;
        var minRanString = reader["MinutesRan"].ToString();
        if (!double.TryParse(minRanString, out var minRan) || name is null)
            continue;

        data.Add(new TrackerData(name, minRan));
    }

    connection.Close();
    var json = JsonSerializer.Serialize(data);
    data.Clear();
    Console.WriteLine("Returning json");
    return json;
}).WithName("GetTrackedData");

app.Run();

internal record TrackerData(string Name, double MinutesRan);