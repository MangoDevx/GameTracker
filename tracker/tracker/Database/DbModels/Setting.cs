using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class Setting
{
    [Key] public int Id { get; set; }

    public string? Name { get; set; }
    public bool IsEnabled { get; set; }
    public string? LastAccessed { get; set; }
}

