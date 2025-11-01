using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Application.Interfaces.Repositories;

public interface IProviderConfigRepository : IRepository<ProviderConfig>
{
    Task<IEnumerable<ProviderConfig>> GetActiveProvidersAsync(CancellationToken cancellationToken = default);
    Task<ProviderConfig?> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default);
}
