using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Providers;
using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Infrastructure.Services;

public class ProviderService : IProviderService
{
    private readonly IContentProviderFactory _providerFactory;
    private readonly IRateLimiter _rateLimiter;

    public ProviderService(IContentProviderFactory providerFactory, IRateLimiter rateLimiter)
    {
        _providerFactory = providerFactory;
        _rateLimiter = rateLimiter;
    }

    public async Task<IEnumerable<Content>> FetchContentFromProviderAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (!await _rateLimiter.IsAllowedAsync(providerId, cancellationToken))
        {
            throw new InvalidOperationException($"Rate limit exceeded for provider {providerId}");
        }

        var provider = _providerFactory.CreateProvider(providerId);
        var content = await provider.FetchContentAsync(cancellationToken);

        await _rateLimiter.RecordRequestAsync(providerId, cancellationToken);

        return content;
    }

    public async Task<IEnumerable<Content>> FetchContentFromAllProvidersAsync(CancellationToken cancellationToken = default)
    {
        var providers = _providerFactory.CreateAllProviders();
        var tasks = new List<Task<IEnumerable<Content>>>();

        foreach (var provider in providers)
        {
            if (await _rateLimiter.IsAllowedAsync(provider.ProviderId, cancellationToken))
            {
                tasks.Add(FetchFromProviderWithLogging(provider, cancellationToken));
            }
        }

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r);
    }

    public async Task<bool> IsProviderAvailableAsync(string providerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var provider = _providerFactory.CreateProvider(providerId);
            return await provider.IsAvailableAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    private async Task<IEnumerable<Content>> FetchFromProviderWithLogging(IContentProvider provider, CancellationToken cancellationToken)
    {
        try
        {
            var content = await provider.FetchContentAsync(cancellationToken);
            await _rateLimiter.RecordRequestAsync(provider.ProviderId, cancellationToken);
            return content;
        }
        catch
        {
            // Log error here
            return Enumerable.Empty<Content>();
        }
    }
}
