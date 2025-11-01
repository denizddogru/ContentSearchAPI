namespace ContentSearchAPI.Application.Interfaces.Providers;

/// <summary>
/// Factory for creating content provider instances based on provider configuration
/// This implements the Factory Pattern for dynamic provider instantiation
/// </summary>
public interface IContentProviderFactory
{
    IContentProvider CreateProvider(string providerId);
    IEnumerable<IContentProvider> CreateAllProviders();
}
