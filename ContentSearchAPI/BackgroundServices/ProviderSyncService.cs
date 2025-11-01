using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Repositories;

namespace ContentSearchAPI.BackgroundServices;

public class ProviderSyncService : BackgroundService
{
    private readonly ILogger<ProviderSyncService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(30);

    public ProviderSyncService(ILogger<ProviderSyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Provider Sync Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncProvidersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while syncing providers");
            }

            await Task.Delay(_syncInterval, stoppingToken);
        }
    }

    private async Task SyncProvidersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var providerService = scope.ServiceProvider.GetRequiredService<IProviderService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var scoringService = scope.ServiceProvider.GetRequiredService<IScoringService>();

        _logger.LogInformation("Fetching content from all providers");

        var contents = await providerService.FetchContentFromAllProvidersAsync(cancellationToken);

        foreach (var content in contents)
        {
            // Calculate scores
            content.BaseScore = scoringService.CalculateBaseScore(content);
            content.RecencyScore = scoringService.CalculateRecencyScore(content.CreatedDate);
            content.InteractionScore = scoringService.CalculateInteractionScore(content);
            content.FinalScore = scoringService.CalculateFinalScore(content);

            await unitOfWork.Contents.AddAsync(content, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully synced {Count} content items", contents.Count());
    }
}
