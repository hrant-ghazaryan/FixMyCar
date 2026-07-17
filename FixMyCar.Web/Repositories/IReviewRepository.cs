using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<List<Review>> GetByTargetUserIdAsync(int targetUserId);
    Task<double> GetAverageRatingAsync(int targetUserId);
    Task<bool> HasUserReviewedAsync(int reviewerId, int targetUserId);
    Task SaveAsync();
}
