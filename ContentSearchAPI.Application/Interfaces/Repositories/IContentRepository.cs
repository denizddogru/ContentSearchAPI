using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Application.Interfaces.Repositories;

public interface IContentRepository : IRepository<Content>
{
    Task<(IEnumerable<Content> Items, int TotalCount)> SearchAsync(
        string? keyword,
        ContentType? contentType,
        SortBy sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
