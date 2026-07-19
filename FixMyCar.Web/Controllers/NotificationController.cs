using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FixMyCar.Web.Services;

namespace FixMyCar.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public class NotificationController(INotificationService notificationService, ILogger<NotificationController> logger) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationController> _logger = logger;

    // GET: api/notification/unread-count
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation($"GetUnreadCount called for UserId: {userId}");
            
            var count = await _notificationService.GetUnreadCountAsync(userId);
            _logger.LogInformation($"Unread count for UserId {userId}: {count}");
            
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetUnreadCount: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/notification/unread
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation($"GetUnread called for UserId: {userId}");
            
            var notifications = await _notificationService.GetUnreadByUserIdAsync(userId);
            _logger.LogInformation($"Found {notifications.Count} unread notifications for UserId {userId}");
            
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetUnread: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/notification/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation($"GetAll called for UserId: {userId}");
            
            var notifications = await _notificationService.GetByUserIdAsync(userId);
            _logger.LogInformation($"Found {notifications.Count} total notifications for UserId {userId}");
            
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetAll: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: api/notification/mark-as-read/{notificationId}
    [HttpPost("mark-as-read/{notificationId}")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            var notification = await _notificationService.GetByIdAsync(notificationId);
            if (notification == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (notification.UserId != userId)
                return Forbid();

            await _notificationService.MarkAsReadAsync(notificationId);
            _logger.LogInformation($"Marked notification {notificationId} as read for UserId {userId}");
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in MarkAsRead: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: api/notification/mark-all-as-read
    [HttpPost("mark-all-as-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _notificationService.MarkAllAsReadAsync(userId);
            _logger.LogInformation($"Marked all notifications as read for UserId {userId}");
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in MarkAllAsRead: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/notification/{notificationId}
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> Delete(int notificationId)
    {
        try
        {
            var notification = await _notificationService.GetByIdAsync(notificationId);
            if (notification == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (notification.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _notificationService.DeleteAsync(notificationId);
            _logger.LogInformation($"Deleted notification {notificationId} for UserId {userId}");
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in Delete: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
