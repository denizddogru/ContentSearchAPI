using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Application.DTOs;

public class ContentSearchRequest
{
    public string? Keyword { get; set; }
    public ContentType? ContentType { get; set; }
    public SortBy SortBy { get; set; } = SortBy.Relevance;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
