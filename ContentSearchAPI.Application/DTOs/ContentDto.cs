using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Application.DTOs;

public class ContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public int? Views { get; set; }
    public int? Likes { get; set; }
    public int? ReadingTime { get; set; }
    public int? Reactions { get; set; }
    public decimal FinalScore { get; set; }
}
