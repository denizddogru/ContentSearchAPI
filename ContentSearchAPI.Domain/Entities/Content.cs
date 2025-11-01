using ContentSearchAPI.Domain.Common;
using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Domain.Entities;

public class Content : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;

    // Video specific properties
    public int? Views { get; set; }
    public int? Likes { get; set; }

    // Text specific properties
    public int? ReadingTime { get; set; }
    public int? Reactions { get; set; }

    // Calculated score fields
    public decimal FinalScore { get; set; }
    public decimal BaseScore { get; set; }
    public decimal RecencyScore { get; set; }
    public decimal InteractionScore { get; set; }
}
