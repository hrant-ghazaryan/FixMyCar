using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface INotificationService
{
    // Get notifications
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);

    // Manage notifications
    Task CreateAsync(Notification notification);
    Task DeleteAsync(int id);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);

    // Create specific notifications
    Task CreateOfferCreatedNotificationAsync(Offer offer);
    Task CreateOfferAcceptedNotificationAsync(Offer offer);
    Task CreateOfferRejectedNotificationAsync(Offer offer);
    Task CreateReviewAddedNotificationAsync(Review review);
    Task CreatePostClosedNotificationAsync(Post post, List<Offer> rejectedOffers);
}
