using NotificationSystem.Brokers;
using NotificationSystem.Models;

namespace NotificationSystem.Handlers;

public class SmsNotificationHandler(IDistributedCache cache, ILogger<SmsNotificationHandler> logger)
{
    public async Task HandleAsync(NotificationEvent notificationEvent)
    {
        if (notificationEvent.NotificationType != "sms")
            return;

        var cacheKey = $"{notificationEvent.UserId}:{notificationEvent.CreatedAt}";
        if (await cache.ContainsAsync(cacheKey))
        {
            logger.LogWarning("Duplicate sms notification detected");
            return;
        }

        await cache.AddAsync(cacheKey, TimeSpan.FromMinutes(5));
        logger.LogInformation($"Sending sms notification to user {notificationEvent.UserId}: {notificationEvent.Message}");
    }
}
