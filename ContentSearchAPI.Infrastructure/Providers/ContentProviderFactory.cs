using ContentSearchAPI.Application.Interfaces.Providers;
using ContentSearchAPI.Application.Interfaces.Repositories;
using ContentSearchAPI.Domain.Enums;

namespace ContentSearchAPI.Infrastructure.Providers;

/// <summary>
/// Factory Pattern implementation for creating content providers
/// Uses Factory Method Pattern to instantiate the correct provider based on configuration
/// </summary>
public class ContentProviderFactory : IContentProviderFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;

    public ContentProviderFactory(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
    }

    public IContentProvider CreateProvider(string providerId)
    {
        var config = _unitOfWork.ProviderConfigs.GetByProviderIdAsync(providerId).GetAwaiter().GetResult();
        if (config == null || !config.IsActive)
        {
            throw new InvalidOperationException($"Provider {providerId} not found or inactive");
        }

        var httpClient = _httpClientFactory.CreateClient(providerId);

        return config.Format switch
        {
            DataFormat.JSON => new JsonContentProvider(httpClient, config),
            DataFormat.XML => new XmlContentProvider(httpClient, config),
            _ => throw new NotSupportedException($"Provider format {config.Format} is not supported")
        };
    }

    public IEnumerable<IContentProvider> CreateAllProviders()
    {
        var activeProviders = _unitOfWork.ProviderConfigs.GetActiveProvidersAsync().GetAwaiter().GetResult();

        foreach (var config in activeProviders)
        {
            var httpClient = _httpClientFactory.CreateClient(config.Id.ToString());

            yield return config.Format switch
            {
                DataFormat.JSON => new JsonContentProvider(httpClient, config),
                DataFormat.XML => new XmlContentProvider(httpClient, config),
                _ => throw new NotSupportedException($"Provider format {config.Format} is not supported")
            };
        }
    }
}
