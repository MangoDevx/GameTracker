using System.ComponentModel.DataAnnotations;

namespace tracker.Database.DbModels;

public class Blacklist
{
    [Key] public int Id { get; set; }

    public string? Name { get; set; }
    public string? Path { get; set; }
}

