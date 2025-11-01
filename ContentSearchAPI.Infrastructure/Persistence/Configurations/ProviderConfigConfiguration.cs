using ContentSearchAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentSearchAPI.Infrastructure.Persistence.Configurations;

public class ProviderConfigConfiguration : IEntityTypeConfiguration<ProviderConfig>
{
    public void Configure(EntityTypeBuilder<ProviderConfig> builder)
    {
        builder.ToTable("ProviderConfigs");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Endpoint)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Format)
            .IsRequired();

        builder.Property(p => p.RequestLimitPerHour)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.HasIndex(p => p.IsActive);
    }
}
