using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class Blacklist
{
    [Key] public int Id { get; set; }

    public string? PathName { get; set; }
    public string? FullPath { get; set; }
}

