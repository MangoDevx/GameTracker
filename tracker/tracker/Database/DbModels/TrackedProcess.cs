using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class TrackedProcess
{
    [Key] public int Id { get; set; }

    public string? ProcessName { get; set; }
    public string? LastAccessed { get; set; }
    public int HoursRan { get; set; }
}
