using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Notification?> GetByIdAsync(int id)
        => await _context.Notifications
            .Include(n => n.User)
            .Include(n => n.RelatedOffer)
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedUser)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
        => await _context.Notifications
            .Include(n => n.RelatedOffer)
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedUser)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
        => await _context.Notifications
            .Include(n => n.RelatedOffer)
            .Include(n => n.RelatedPost)
            .Include(n => n.RelatedUser)
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(int userId)
        => await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();

    public async Task AddAsync(Notification notification)
        => await _context.Notifications.AddAsync(notification);

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
            _context.Notifications.Remove(notification);
    }

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            await SaveAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await GetUnreadByUserIdAsync(userId);
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }
        await SaveAsync();
    }
}
