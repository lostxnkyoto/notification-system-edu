using NotificationSystem.Brokers;
using NotificationSystem.Models;

namespace NotificationSystem.Handlers;

public class EmailNotificationHandler(IDistributedCache cache, ILogger<EmailNotificationHandler> logger)
{
    public async Task HandleAsync(NotificationEvent notificationEvent)
    {
        if (notificationEvent.NotificationType != "email")
            return;

        var cacheKey = $"{notificationEvent.UserId}:{notificationEvent.CreatedAt}";
        if (await cache.ContainsAsync(cacheKey))
        {
            logger.LogWarning("Duplicate email notification detected");
            return;
        }

        await cache.AddAsync(cacheKey, TimeSpan.FromMinutes(5));
        logger.LogInformation($"Sending email notification to user {notificationEvent.UserId}: {notificationEvent.Message}");
    }
}