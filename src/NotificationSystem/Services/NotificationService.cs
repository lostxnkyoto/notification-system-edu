using NotificationSystem.Brokers;
using NotificationSystem.Models;

namespace NotificationSystem.Services;

public class NotificationService(IMessageBroker messageBroker, ILogger<NotificationService> logger)
{
    public async Task NotifyUserAsync(string userId, string message, string notificationType)
    {
        var notificationEvent = new NotificationEvent
        {
            UserId = userId,
            Message = message,
            CreatedAt = DateTime.Now,
            NotificationType = notificationType
        };

        logger.LogInformation($"Creating new notification {notificationType} for the user {userId}");
        await messageBroker.PublishAsync(notificationEvent);
    }
}