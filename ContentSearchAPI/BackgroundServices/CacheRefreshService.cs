using ContentSearchAPI.Application.Interfaces;

namespace ContentSearchAPI.BackgroundServices;

public class CacheRefreshService : BackgroundService
{
    private readonly ILogger<CacheRefreshService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(20);

    public CacheRefreshService(ILogger<CacheRefreshService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
   
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache Refresh Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshCacheAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing cache");
            }

            await Task.Delay(_refreshInterval, stoppingToken);
        }
    }

    private async Task RefreshCacheAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        _logger.LogInformation("Cache refresh completed");

        // Additional cache refresh logic can be added here
        await Task.CompletedTask;
    }
}
