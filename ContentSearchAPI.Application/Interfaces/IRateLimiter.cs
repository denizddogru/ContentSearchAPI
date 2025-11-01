namespace ContentSearchAPI.Application.Interfaces;

public interface IRateLimiter
{
    Task<bool> IsAllowedAsync(string providerId, CancellationToken cancellationToken = default);
    Task RecordRequestAsync(string providerId, CancellationToken cancellationToken = default);
}
