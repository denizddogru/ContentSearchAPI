using ContentSearchAPI.Application.Interfaces.Providers;
using ContentSearchAPI.Domain.Entities;

namespace ContentSearchAPI.Infrastructure.Providers;

/// <summary>
/// Base class for content providers implementing common functionality
/// Uses Template Method Pattern
/// </summary>
public abstract class BaseContentProvider : IContentProvider
{
    protected readonly HttpClient _httpClient;
    protected readonly ProviderConfig _config;

    protected BaseContentProvider(HttpClient httpClient, ProviderConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public string ProviderId => _config.Id.ToString();

    public abstract Task<IEnumerable<Content>> FetchContentAsync(CancellationToken cancellationToken = default);

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(_config.Endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    protected abstract Task<IEnumerable<Content>> ParseResponseAsync(string response, CancellationToken cancellationToken);
}
