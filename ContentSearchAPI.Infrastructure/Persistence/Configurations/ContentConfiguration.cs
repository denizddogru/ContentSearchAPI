using ContentSearchAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContentSearchAPI.Infrastructure.Persistence.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("Contents");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.ProviderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.SourceUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.FinalScore)
            .HasPrecision(18, 2);

        builder.Property(c => c.BaseScore)
            .HasPrecision(18, 2);

        builder.Property(c => c.RecencyScore)
            .HasPrecision(18, 2);

        builder.Property(c => c.InteractionScore)
            .HasPrecision(18, 2);

        // Indexes for search performance
        builder.HasIndex(c => c.Title);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.FinalScore);
        builder.HasIndex(c => c.CreatedDate);
        builder.HasIndex(c => c.ProviderId);
    }
}
