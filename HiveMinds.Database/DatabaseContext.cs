using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HiveMinds.Database;

public class DatabaseContext : DbContext
{
    public required DbSet<Account> Account { get; set; }
    public required DbSet<Thought> Thought { get; set; }
    public required DbSet<VerificationRequest> VerificationRequest { get; set; }
    public required DbSet<ThoughtLike> ThoughtLike { get; set; }
    public required DbSet<ThoughtReply> ThoughtReply { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options, ILogger<DatabaseContext> logger) : base(options)
    {
        logger.LogInformation("DatabaseContext created");
    }
}