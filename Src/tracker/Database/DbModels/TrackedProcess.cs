using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class TrackedProcess
{
    [Key] public int Id { get; set; }

    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? LastAccessed { get; set; }
    public bool Tracking { get; set; } = true;
    public double MinutesRan { get; set; }
}
