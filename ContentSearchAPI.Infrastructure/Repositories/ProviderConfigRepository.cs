using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.Domain.Entities;
using ContentSearchAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContentSearchAPI.Infrastructure.Repositories;

public class ProviderConfigRepository : Repository<ProviderConfig>, IProviderConfigRepository
{
    public ProviderConfigRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProviderConfig>> GetActiveProvidersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<ProviderConfig?> GetByProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id.ToString() == providerId, cancellationToken);
    }
}
