using ContentSearchAPI.Application.Interfaces;
using ContentSearchAPI.Application.Interfaces.Repositories;

namespace ContentSearchAPI.BackgroundServices;

public class ScoreCalculationService : BackgroundService
{
    private readonly ILogger<ScoreCalculationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _calculationInterval = TimeSpan.FromHours(1);

    public ScoreCalculationService(ILogger<ScoreCalculationService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Score Calculation Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RecalculateScoresAsync(stoppingToken);
                await Task.Delay(_calculationInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when application is shutting down
                _logger.LogInformation("Score Calculation Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while recalculating scores");

                // Wait a bit before retrying on error (but shorter interval)
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Score Calculation Service has stopped");
    }

    private async Task RecalculateScoresAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var scoringService = scope.ServiceProvider.GetRequiredService<IScoringService>();

        _logger.LogInformation("Recalculating scores for all content");

        var allContent = await unitOfWork.Contents.GetAllAsync(cancellationToken);

        foreach (var content in allContent)
        {
            content.BaseScore = scoringService.CalculateBaseScore(content);
            content.RecencyScore = scoringService.CalculateRecencyScore(content.CreatedDate);
            content.InteractionScore = scoringService.CalculateInteractionScore(content);
            content.FinalScore = scoringService.CalculateFinalScore(content);

            await unitOfWork.Contents.UpdateAsync(content, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully recalculated scores for {Count} items", allContent.Count());
    }
}
