using KMITL_WebDev_MiniProject.Entites;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Services;

public class ClosedActivityCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ClosedActivityCleanupService> _logger;

    public ClosedActivityCleanupService(IServiceScopeFactory scopeFactory, ILogger<ClosedActivityCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run once shortly after startup, then run every 12 hours.
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupClosedActivitiesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }

    private async Task CleanupClosedActivitiesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationActivitiesDbContext>();

            var cutoff = DateTime.Now.AddDays(-60);

            var oldClosedActivities = await db.Activities
                .Where(a => a.EventDate <= cutoff)
                .ToListAsync(cancellationToken);

            if (oldClosedActivities.Count == 0)
                return;

            db.Activities.RemoveRange(oldClosedActivities);
            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} closed activities older than 60 days.", oldClosedActivities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while cleaning up old closed activities.");
        }
    }
}
