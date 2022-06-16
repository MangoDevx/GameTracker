using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class Whitelist
{
    [Key] public int Id { get; set; }

    public string? PathName { get; set; }
    public string? FullPath { get; set; }
}
