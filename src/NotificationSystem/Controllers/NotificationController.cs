using Microsoft.AspNetCore.Mvc;
using NotificationSystem.Services;

namespace NotificationSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(NotificationService notificationService, ILogger<NotificationController> logger)
    : ControllerBase
{
    [HttpPost("notify")]
    public async Task<IActionResult> Notify([FromQuery] string userId, [FromQuery] string message, [FromQuery] string notificationType)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(notificationType))
        {
            logger.LogWarning("Invalid request: missing parameters");
            return BadRequest("userId, message, and notificationType are required");
        }

        await notificationService.NotifyUserAsync(userId, message, notificationType);
        return Ok("Notification sent");
    }
}