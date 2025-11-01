using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Application.Interfaces.Providers;

public interface IContentProvider
{
    string ProviderId { get; }
    Task<IEnumerable<Content>> FetchContentAsync(CancellationToken cancellationToken = default);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
