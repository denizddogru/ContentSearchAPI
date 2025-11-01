using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;
using ContentSearchAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentSearchAPI.Infrastructure.Repositories;

public class ContentRepository : Repository<Content>, IContentRepository
{
    public ContentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Content> Items, int TotalCount)> SearchAsync(
        string? keyword,
        ContentType? contentType,
        SortBy sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Filter by keyword
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword));
        }

        // Filter by content type
        if (contentType.HasValue)
        {
            query = query.Where(c => c.Type == contentType.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy switch
        {
            SortBy.Popularity => query.OrderByDescending(c => c.FinalScore),
            SortBy.Relevance => query.OrderByDescending(c => c.FinalScore).ThenByDescending(c => c.CreatedDate),
            _ => query.OrderByDescending(c => c.CreatedDate)
        };

        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
