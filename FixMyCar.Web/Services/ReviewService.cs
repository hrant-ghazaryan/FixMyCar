using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;

namespace FixMyCar.Web.Services;

public class ReviewService(IReviewRepository reviewRepository, INotificationService? notificationService = null) : IReviewService
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;
    private readonly INotificationService? _notificationService = notificationService;

    public async Task AddReviewAsync(Review review)
    {
        if (review.ReviewerId == review.TargetUserId)
            throw new InvalidOperationException("You cannot review yourself.");

        if (review.Rating < 1 || review.Rating > 5)
            throw new ArgumentOutOfRangeException(nameof(review.Rating), "Rating must be between 1 and 5.");

        var hasReviewed = await _reviewRepository.HasUserReviewedAsync(review.ReviewerId, review.TargetUserId);
        if (hasReviewed)
            throw new InvalidOperationException("You have already reviewed this user.");

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveAsync();

        // 🔔 Send notification to target user
        if (_notificationService != null)
        {
            await _notificationService.CreateReviewAddedNotificationAsync(review);
        }
    }

    public Task<List<Review>> GetReviewsForUserAsync(int targetUserId)
        => _reviewRepository.GetByTargetUserIdAsync(targetUserId);

    public Task<double> GetAverageRatingForUserAsync(int targetUserId)
        => _reviewRepository.GetAverageRatingAsync(targetUserId);

    public Task<bool> HasUserReviewedAsync(int reviewerId, int targetUserId)
        => _reviewRepository.HasUserReviewedAsync(reviewerId, targetUserId);
}
