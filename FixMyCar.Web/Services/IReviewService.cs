using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface IReviewService
{
    Task AddReviewAsync(Review review);
    Task<List<Review>> GetReviewsForUserAsync(int targetUserId);
    Task<double> GetAverageRatingForUserAsync(int targetUserId);
    Task<bool> HasUserReviewedAsync(int reviewerId, int targetUserId);
}
