using NotificationSystem.Models;

namespace NotificationSystem.Brokers;

public interface IMessageBroker
{
    Task PublishAsync(NotificationEvent notificationEvent);
    void Subscribe(string queueName, Func<NotificationEvent, Task> onMessageReceived);
}