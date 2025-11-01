using ContentSearchAPI.Application.DTOs;

namespace ContentSearchAPI.Application.Interfaces;

public interface IContentService
{
    Task<PagedResult<ContentDto>> SearchAsync(ContentSearchRequest request, CancellationToken cancellationToken = default);
    Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
