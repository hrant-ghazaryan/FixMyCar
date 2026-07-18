using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using FixMyCar.Web.Services;
using Xunit;

namespace FixMyCar.Web.Tests;

public class ReviewServiceTests
{
    [Fact]
    public async Task AddReviewAsync_RejectsSecondReviewForSameUserPair()
    {
        var repository = new FakeReviewRepository { HasReview = true };
        var service = new ReviewService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddReviewAsync(new Review
        {
            ReviewerId = 1,
            TargetUserId = 2,
            Rating = 5,
            Comment = "Great service"
        }));
    }

    [Fact]
    public async Task AddReviewAsync_RejectsSelfReview()
    {
        var service = new ReviewService(new FakeReviewRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddReviewAsync(new Review
        {
            ReviewerId = 1,
            TargetUserId = 1,
            Rating = 5,
            Comment = "Invalid"
        }));
    }

    [Fact]
    public async Task AddReviewAsync_SavesValidReview()
    {
        var repository = new FakeReviewRepository();
        var service = new ReviewService(repository);
        var review = new Review { ReviewerId = 1, TargetUserId = 2, Rating = 4, Comment = "Good" };

        await service.AddReviewAsync(review);

        Assert.Same(review, repository.AddedReview);
        Assert.True(repository.SaveCalled);
    }

    private sealed class FakeReviewRepository : IReviewRepository
    {
        public bool HasReview { get; init; }
        public Review? AddedReview { get; private set; }
        public bool SaveCalled { get; private set; }
        public Task AddAsync(Review review) { AddedReview = review; return Task.CompletedTask; }
        public Task<List<Review>> GetByTargetUserIdAsync(int targetUserId) => Task.FromResult(new List<Review>());
        public Task<double> GetAverageRatingAsync(int targetUserId) => Task.FromResult(0d);
        public Task<bool> HasUserReviewedAsync(int reviewerId, int targetUserId) => Task.FromResult(HasReview);
        public Task SaveAsync() { SaveCalled = true; return Task.CompletedTask; }
    }
}
