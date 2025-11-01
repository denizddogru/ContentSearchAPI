using ContentSearchAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentSearchAPI.Infrastructure.Persistence.Configurations;

public class ProviderRequestLogConfiguration : IEntityTypeConfiguration<ProviderRequestLog>
{
    public void Configure(EntityTypeBuilder<ProviderRequestLog> builder)
    {
        builder.ToTable("ProviderRequestLogs");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProviderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.RequestTimestamp)
            .IsRequired();

        builder.Property(p => p.IsSuccessful)
            .IsRequired();

        builder.Property(p => p.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(p => new { p.ProviderId, p.RequestTimestamp });
    }
}
