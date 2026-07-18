using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using FixMyCar.Web.Services;
using Xunit;

namespace FixMyCar.Web.Tests;

public class OfferServiceTests
{
    [Fact]
    public async Task UpdateAsync_RejectsRequestFromAnotherUser()
    {
        var repository = new FakeOfferRepository(new Offer { Id = 1, UserId = 10, Price = 20_000, PostId = 1, Post = new Post { UserId = 20 } });
        var service = new OfferService(repository);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.UpdateAsync(1, 11, 25_000, "Updated"));
    }

    [Fact]
    public async Task DeleteAsync_RejectsAcceptedOffer()
    {
        var repository = new FakeOfferRepository(new Offer { Id = 1, UserId = 10, Status = OfferStatus.Accepted, PostId = 1, Post = new Post { UserId = 20 } });
        var service = new OfferService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1, 10));
    }

    [Fact]
    public async Task AcceptOfferAsync_AcceptsChosenOffer_ClosesPost_AndRejectsOtherPendingOffers()
    {
        var post = new Post { Id = 1, UserId = 20 };
        var selected = new Offer { Id = 1, UserId = 10, PostId = 1, Post = post, Status = OfferStatus.Pending };
        var other = new Offer { Id = 2, UserId = 11, PostId = 1, Post = post, Status = OfferStatus.Pending };
        var repository = new FakeOfferRepository(selected, other);
        var service = new OfferService(repository);

        await service.AcceptOfferAsync(selected.Id, post.UserId);

        Assert.Equal(OfferStatus.Accepted, selected.Status);
        Assert.Equal(OfferStatus.Rejected, other.Status);
        Assert.NotNull(post.ClosedAt);
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public async Task AcceptOfferAsync_RejectsNonOwner()
    {
        var offer = new Offer { Id = 1, UserId = 10, PostId = 1, Post = new Post { UserId = 20 } };
        var service = new OfferService(new FakeOfferRepository(offer));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.AcceptOfferAsync(offer.Id, 99));
    }

    private sealed class FakeOfferRepository(params Offer[] offers) : IOfferRepository
    {
        private readonly List<Offer> _offers = offers.ToList();
        public bool SaveCalled { get; private set; }

        public Task<List<Offer>> GetAllAsync() => Task.FromResult(_offers.ToList());
        public Task<Offer?> GetByIdAsync(int id) => Task.FromResult(_offers.SingleOrDefault(x => x.Id == id));
        public Task<List<Offer>> GetByPostIdAsync(int postId) => Task.FromResult(_offers.Where(x => x.PostId == postId).ToList());
        public Task AddAsync(Offer offer) { _offers.Add(offer); return Task.CompletedTask; }
        public void Delete(Offer offer) => _offers.Remove(offer);
        public Task<List<Offer>> GetByUserIdAsync(int userId) => Task.FromResult(_offers.Where(x => x.UserId == userId).ToList());
        public Task SaveAsync() { SaveCalled = true; return Task.CompletedTask; }
    }
}
