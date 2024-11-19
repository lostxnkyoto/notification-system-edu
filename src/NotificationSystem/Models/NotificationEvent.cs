namespace NotificationSystem.Models;

public class NotificationEvent
{
    public string UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string NotificationType { get; set; } // e.g., "email", "sms"
}