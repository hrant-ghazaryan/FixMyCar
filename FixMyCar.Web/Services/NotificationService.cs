using FixMyCar.Web.Hubs;
using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace FixMyCar.Web.Services;

public class NotificationService(INotificationRepository notificationRepository, 
    IUserService userService, IPostService postService,
    IHubContext<NotificationHub> hubContext) : INotificationService
{
    private readonly INotificationRepository _repository = notificationRepository;
    private readonly IUserService _userService = userService;
    private readonly IPostService _postService = postService;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

    public async Task<Notification?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
        => await _repository.GetByUserIdAsync(userId);

    public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
        => await _repository.GetUnreadByUserIdAsync(userId);

    public async Task<int> GetUnreadCountAsync(int userId)
        => await _repository.GetUnreadCountAsync(userId);

    public async Task CreateAsync(Notification notification)
    {
        await _repository.AddAsync(notification);
        await _repository.SaveAsync();

        var unreadCount = await _repository.GetUnreadCountAsync(notification.UserId);
        await _hubContext.Clients.Group($"user-{notification.UserId}")
            .SendAsync("UnreadCountUpdated", unreadCount);

        await _hubContext.Clients.Group($"user-{notification.UserId}")
            .SendAsync("NotificationCreated", notification);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        await _repository.SaveAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
        => await _repository.MarkAsReadAsync(notificationId);

    public async Task MarkAllAsReadAsync(int userId)
        => await _repository.MarkAllAsReadAsync(userId);

    // Specific notification creators
    public async Task CreateOfferCreatedNotificationAsync(Offer offer)
    {
        var offerUser = await _userService.GetByIdAsync(offer.UserId);
        if (offerUser == null) return;

        var post = offer.Post ?? await _postService.GetByIdAsync(offer.PostId);
        if (post == null) return;

        var notification = new Notification
        {
            UserId = post.UserId,
            Type = NotificationType.OfferCreated,
            Title = "New Offer",
            Message = $"{offerUser.DisplayName} sent an offer for ֏{offer.Price:N0} {offer.Currency}",
            RelatedOfferId = offer.Id,
            RelatedPostId = offer.PostId,
            RelatedUserId = offer.UserId,
            IsRead = false
        };

        await CreateAsync(notification);
    }

    public async Task CreateOfferAcceptedNotificationAsync(Offer offer)
    {
        var post = offer.Post ?? await _postService.GetByIdAsync(offer.PostId);
        if (post == null) return;

        var postUser = await _userService.GetByIdAsync(post.UserId);
        if (postUser == null) return;

        var notification = new Notification
        {
            UserId = offer.UserId,
            Type = NotificationType.OfferAccepted,
            Title = "Offer Accepted ✅",
            Message = $"{postUser.DisplayName} accepted your offer for {post.Title}",
            RelatedOfferId = offer.Id,
            RelatedPostId = offer.PostId,
            RelatedUserId = post.UserId,
            IsRead = false
        };

        await CreateAsync(notification);
    }

    public async Task CreateOfferRejectedNotificationAsync(Offer offer)
    {
        var post = offer.Post ?? await _postService.GetByIdAsync(offer.PostId);
        if (post == null) return;

        var notification = new Notification
        {
            UserId = offer.UserId,
            Type = NotificationType.OfferRejected,
            Title = "Offer Rejected",
            Message = $"Your offer for \"{post.Title}\" was rejected",
            RelatedOfferId = offer.Id,
            RelatedPostId = offer.PostId,
            RelatedUserId = post.UserId,
            IsRead = false
        };

        await CreateAsync(notification);
    }

    public async Task CreateReviewAddedNotificationAsync(Review review)
    {
        var reviewer = await _userService.GetByIdAsync(review.ReviewerId);
        if (reviewer == null) return;

        var notification = new Notification
        {
            UserId = review.TargetUserId,
            Type = NotificationType.ReviewAdded,
            Title = $"New Review: {review.Rating}⭐",
            Message = $"{reviewer.DisplayName} left a {review.Rating}-star review",
            RelatedUserId = review.ReviewerId,
            IsRead = false
        };

        await CreateAsync(notification);
    }

    public async Task CreatePostClosedNotificationAsync(Post post, List<Offer> rejectedOffers)
    {
        foreach (var offer in rejectedOffers)
        {
            await CreateOfferRejectedNotificationAsync(offer);
        }
    }
}
