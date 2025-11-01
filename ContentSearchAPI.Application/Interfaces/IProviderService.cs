using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Application.Interfaces;

public interface IProviderService
{
    Task<IEnumerable<Content>> FetchContentFromProviderAsync(string providerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Content>> FetchContentFromAllProvidersAsync(CancellationToken cancellationToken = default);
    Task<bool> IsProviderAvailableAsync(string providerId, CancellationToken cancellationToken = default);
}
