using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Review review)
        => await _context.Reviews.AddAsync(review);

    public async Task<List<Review>> GetByTargetUserIdAsync(int targetUserId)
        => await _context.Reviews
            .Where(r => r.TargetUserId == targetUserId)
            .Include(r => r.Reviewer)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<double> GetAverageRatingAsync(int targetUserId)
    {
        var ratings = await _context.Reviews
            .Where(r => r.TargetUserId == targetUserId)
            .Select(r => r.Rating)
            .ToListAsync();

        return ratings.Any() ? ratings.Average() : 0.0;
    }

    public async Task<bool> HasUserReviewedAsync(int reviewerId, int targetUserId)
        => await _context.Reviews
            .AnyAsync(r => r.ReviewerId == reviewerId && r.TargetUserId == targetUserId);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();
}
