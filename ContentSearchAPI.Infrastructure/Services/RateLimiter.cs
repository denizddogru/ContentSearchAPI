using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ContentSearchAPI.Infrastructure.Services;

/// <summary>
/// Sliding window rate limiter implementation
/// Singleton service that uses IServiceProvider to create scopes for database access
/// </summary>
public class RateLimiter : IRateLimiter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Queue<DateTime>> _requestWindows = new();
    private readonly Dictionary<string, int> _providerLimitsCache = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private DateTime _lastCacheRefresh = DateTime.MinValue;
    private readonly TimeSpan _cacheRefreshInterval = TimeSpan.FromMinutes(10);

    public RateLimiter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> IsAllowedAsync(string providerId, CancellationToken cancellationToken = default)
    {
        var limit = await GetProviderLimitAsync(providerId, cancellationToken);
        if (limit == 0)
            return false;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!_requestWindows.ContainsKey(providerId))
            {
                _requestWindows[providerId] = new Queue<DateTime>();
            }

            var window = _requestWindows[providerId];
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);

            // Remove requests older than 1 hour
            while (window.Count > 0 && window.Peek() < oneHourAgo)
            {
                window.Dequeue();
            }

            return window.Count < limit;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RecordRequestAsync(string providerId, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (!_requestWindows.ContainsKey(providerId))
            {
                _requestWindows[providerId] = new Queue<DateTime>();
            }

            _requestWindows[providerId].Enqueue(DateTime.UtcNow);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<int> GetProviderLimitAsync(string providerId, CancellationToken cancellationToken)
    {
        // Refresh cache if needed
        if (DateTime.UtcNow - _lastCacheRefresh > _cacheRefreshInterval)
        {
            await RefreshCacheAsync(cancellationToken);
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            return _providerLimitsCache.GetValueOrDefault(providerId, 0);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task RefreshCacheAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var providers = await unitOfWork.ProviderConfigs.GetActiveProvidersAsync(cancellationToken);

        await _lock.WaitAsync(cancellationToken);
        try
        {
            _providerLimitsCache.Clear();
            foreach (var provider in providers)
            {
                _providerLimitsCache[provider.Id.ToString()] = provider.RequestLimitPerHour;
            }
            _lastCacheRefresh = DateTime.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }
}
