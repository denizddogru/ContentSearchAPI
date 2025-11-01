using ContentSearchAPI.Domain.Common;
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set IAuditable fields
        var entries = ChangeTracker.Entries<IAuditable>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "System"; // TODO: Get from current user context
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = "System"; // TODO: Get from current user context
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
