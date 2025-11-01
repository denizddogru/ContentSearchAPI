using ContentSearchAPI.Application.DTOs;
using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Repositories;

namespace ContentSearchAPI.Infrastructure.Services;

public class ContentService : IContentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ContentService(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<ContentDto>> SearchAsync(ContentSearchRequest request, CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = $"search:{request.Keyword}:{request.ContentType}:{request.SortBy}:{request.Page}:{request.PageSize}";

        // Try to get from cache
        var cachedResult = await _cacheService.GetAsync<PagedResult<ContentDto>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // Search from database
        var (items, totalCount) = await _unitOfWork.Contents.SearchAsync(
            request.Keyword,
            request.ContentType,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);

        var result = new PagedResult<ContentDto>
        {
            Items = items.Select(c => new ContentDto
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type,
                Description = c.Description,
                CreatedDate = c.CreatedDate,
                ProviderId = c.ProviderId,
                SourceUrl = c.SourceUrl,
                Views = c.Views,
                Likes = c.Likes,
                ReadingTime = c.ReadingTime,
                Reactions = c.Reactions,
                FinalScore = c.FinalScore
            }),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        // Cache the result for 15 minutes
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);

        return result;
    }

    public async Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(id, cancellationToken);
        if (content == null)
            return null;

        return new ContentDto
        {
            Id = content.Id,
            Title = content.Title,
            Type = content.Type,
            Description = content.Description,
            CreatedDate = content.CreatedDate,
            ProviderId = content.ProviderId,
            SourceUrl = content.SourceUrl,
            Views = content.Views,
            Likes = content.Likes,
            ReadingTime = content.ReadingTime,
            Reactions = content.Reactions,
            FinalScore = content.FinalScore
        };
    }
}
