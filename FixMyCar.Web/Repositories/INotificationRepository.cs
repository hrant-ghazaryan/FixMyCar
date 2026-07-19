using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task DeleteAsync(int id);
    Task SaveAsync();
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}
