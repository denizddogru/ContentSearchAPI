using ContentSearchAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContentSearchAPI.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Content> Contents => Set<Content>();
    public DbSet<ProviderConfig> ProviderConfigs => Set<ProviderConfig>();
    public DbSet<ProviderRequestLog> ProviderRequestLogs => Set<ProviderRequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
