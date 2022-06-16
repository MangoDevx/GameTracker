using Microsoft.EntityFrameworkCore;
using tracker.Database.DbModels;

namespace tracker.Database;

public class DataContext : DbContext
{
    public DbSet<TrackedProcess> Processes { get; set; } = null!;
    public DbSet<Whitelist> Whitelists { get; set; } = null!;
    public DbSet<Blacklist> Blacklists { get; set; } = null!;

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
}
 