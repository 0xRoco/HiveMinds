using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HiveMinds.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Account> Account { get; set; }
    public DbSet<Thought> Thought { get; set; }
    public DbSet<VerificationRequest> VerificationRequest { get; set; }
    public DbSet<ThoughtLike> ThoughtLike { get; set; }
    public DbSet<ThoughtReply> ThoughtReply { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger) : base(options)
    {
        logger.LogInformation("DatabaseContext created");
    }
}