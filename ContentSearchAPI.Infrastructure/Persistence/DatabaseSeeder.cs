using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContentSearchAPI.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if database has been seeded
            if (await _context.Contents.AnyAsync())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed Provider Configurations
            var providers = new[]
            {
                new ProviderConfig
                {
                    Id = Guid.NewGuid(),
                    Name = "JSON Provider",
                    Endpoint = "http://localhost:5000/mock/provider1/content",
                    Format = DataFormat.JSON,
                    RequestLimitPerHour = 100,
                    IsActive = true
                },
                new ProviderConfig
                {
                    Id = Guid.NewGuid(),
                    Name = "XML Provider",
                    Endpoint = "http://localhost:5000/mock/provider2/data",
                    Format = DataFormat.XML,
                    RequestLimitPerHour = 50,
                    IsActive = true
                }
            };

            await _context.ProviderConfigs.AddRangeAsync(providers);
            await _context.SaveChangesAsync();

            // Seed Sample Content
            var contents = new[]
            {
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Getting Started with .NET 9",
                    Description = "Learn the new features and improvements in .NET 9",
                    Type = ContentType.Video,
                    ProviderId = providers[0].Id.ToString(),
                    SourceUrl = "https://example.com/video1",
                    Views = 15000,
                    Likes = 1200,
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    BaseScore = 27,
                    RecencyScore = 5,
                    InteractionScore = 0.8m,
                    FinalScore = 46.3m
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Clean Architecture Principles",
                    Description = "Understanding Clean Architecture in modern applications",
                    Type = ContentType.Text,
                    ProviderId = providers[1].Id.ToString(),
                    SourceUrl = "https://example.com/article1",
                    ReadingTime = 15,
                    Reactions = 450,
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    BaseScore = 24,
                    RecencyScore = 3,
                    InteractionScore = 150,
                    FinalScore = 177
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Microservices Architecture Patterns",
                    Description = "Building scalable microservices with .NET",
                    Type = ContentType.Video,
                    ProviderId = providers[0].Id.ToString(),
                    SourceUrl = "https://example.com/video2",
                    Views = 25000,
                    Likes = 2100,
                    CreatedDate = DateTime.UtcNow.AddDays(-3),
                    BaseScore = 46,
                    RecencyScore = 5,
                    InteractionScore = 0.84m,
                    FinalScore = 74.76m
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Design Patterns in C#",
                    Description = "Comprehensive guide to design patterns",
                    Type = ContentType.Text,
                    ProviderId = providers[1].Id.ToString(),
                    SourceUrl = "https://example.com/article2",
                    ReadingTime = 20,
                    Reactions = 680,
                    CreatedDate = DateTime.UtcNow.AddDays(-7),
                    BaseScore = 33.6m,
                    RecencyScore = 5,
                    InteractionScore = 170,
                    FinalScore = 208.6m
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Entity Framework Core Best Practices",
                    Description = "Optimize your EF Core applications",
                    Type = ContentType.Video,
                    ProviderId = providers[0].Id.ToString(),
                    SourceUrl = "https://example.com/video3",
                    Views = 8000,
                    Likes = 650,
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    BaseScore = 14.5m,
                    RecencyScore = 3,
                    InteractionScore = 0.81m,
                    FinalScore = 25.96m
                }
            };

            await _context.Contents.AddRangeAsync(contents);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully. Added {ProviderCount} providers and {ContentCount} content items",
                providers.Length, contents.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
