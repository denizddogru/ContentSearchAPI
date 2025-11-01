using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Application.Interfaces;

public interface IScoringService
{
    decimal CalculateFinalScore(Content content);
    decimal CalculateBaseScore(Content content);
    decimal CalculateRecencyScore(DateTime createdDate);
    decimal CalculateInteractionScore(Content content);
}
