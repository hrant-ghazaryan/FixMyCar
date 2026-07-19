using Microsoft.AspNetCore.SignalR;
using FixMyCar.Web.Services;

namespace FixMyCar.Web.Hubs;

public class NotificationHub(INotificationService notificationService) : Hub
{
    private readonly INotificationService _notificationService = notificationService;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to a group named after their userId
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    // Client can call this to get unread count
    public async Task GetUnreadCount()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
        {
            var count = await _notificationService.GetUnreadCountAsync(userIdInt);
            await Clients.Caller.SendAsync("UnreadCountUpdated", count);
        }
    }

    // Client can call this to get unread notifications
    public async Task GetUnreadNotifications()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
        {
            var notifications = await _notificationService.GetUnreadByUserIdAsync(userIdInt);
            await Clients.Caller.SendAsync("UnreadNotificationsReceived", notifications);
        }
    }

    // Called when user marks a notification as read
    public async Task MarkAsRead(int notificationId)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
        {
            var notification = await _notificationService.GetByIdAsync(notificationId);
            if (notification != null && notification.UserId == userIdInt)
            {
                await _notificationService.MarkAsReadAsync(notificationId);
                
                // Notify client
                var count = await _notificationService.GetUnreadCountAsync(userIdInt);
                await Clients.Group($"user-{userId}").SendAsync("UnreadCountUpdated", count);
            }
        }
    }
}
