using ContentSearchAPI.Application.Interfaces;
using System.Text.Json;

namespace ContentSearchAPI.Infrastructure.Services;

/// <summary>
/// In-memory cache service implementation
/// Can be replaced with Redis implementation for distributed caching
/// </summary>
public class CacheService : ICacheService
{
    private readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (cached.Expiration > DateTime.UtcNow)
                {
                    return cached.Value as T;
                }
                else
                {
                    _cache.Remove(key);
                }
            }
            return null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var expirationTime = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(15));
            _cache[key] = (value, expirationTime);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            _cache.Remove(key);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (cached.Expiration > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    _cache.Remove(key);
                }
            }
            return false;
        }
        finally
        {
            _lock.Release();
        }
    }
}
