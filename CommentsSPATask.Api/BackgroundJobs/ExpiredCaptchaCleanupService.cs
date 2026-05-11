using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;

namespace CommentsSPATask.Api.BackgroundJobs;

public sealed class ExpiredCaptchaCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredCaptchaCleanupService> logger)
    : BackgroundService
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ExpiredRetention = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunCleanupAsync(stoppingToken);

        using var timer = new PeriodicTimer(CleanupInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCleanupAsync(stoppingToken);
        }
    }

    private async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var captchaRepository = scope.ServiceProvider.GetRequiredService<ICaptchaRepository>();
            var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var removedCount = await captchaRepository.RemoveExpiredCaptchasAsync(
                dateTimeProvider.UtcNow.Subtract(ExpiredRetention),
                cancellationToken);

            if (removedCount == 0)
            {
                return;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Removed {RemovedCount} expired captcha records older than {Retention}",
                removedCount,
                ExpiredRetention);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Expired captcha cleanup job failed");
        }
    }
}
