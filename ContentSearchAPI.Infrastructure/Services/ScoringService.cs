using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Infrastructure.Services;

public class ScoringService : IScoringService
{
    public decimal CalculateFinalScore(Content content)
    {
        var baseScore = CalculateBaseScore(content);
        var contentTypeMultiplier = content.Type == ContentType.Video ? 1.5m : 1.0m;
        var recencyScore = CalculateRecencyScore(content.CreatedDate);
        var interactionScore = CalculateInteractionScore(content);

        return (baseScore * contentTypeMultiplier) + recencyScore + interactionScore;
    }

    public decimal CalculateBaseScore(Content content)
    {
        return content.Type switch
        {
            ContentType.Video => (content.Views ?? 0) / 1000m + (content.Likes ?? 0) / 100m,
            ContentType.Text => (content.ReadingTime ?? 0) + (content.Reactions ?? 0) / 50m,
            _ => 0
        };
    }

    public decimal CalculateRecencyScore(DateTime createdDate)
    {
        var age = DateTime.UtcNow - createdDate;

        if (age.TotalDays <= 7)
            return 5;
        if (age.TotalDays <= 30)
            return 3;
        if (age.TotalDays <= 90)
            return 1;

        return 0;
    }

    public decimal CalculateInteractionScore(Content content)
    {
        return content.Type switch
        {
            ContentType.Video => content.Views > 0 ? ((content.Likes ?? 0) / (decimal)content.Views.Value) * 10 : 0,
            ContentType.Text => content.ReadingTime > 0 ? ((content.Reactions ?? 0) / (decimal)content.ReadingTime.Value) * 5 : 0,
            _ => 0
        };
    }
}
