using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;

namespace FixMyCar.Web.Services;

public class OfferService(IOfferRepository repo) : IOfferService
{
    private readonly IOfferRepository _repo = repo;

    public async Task<List<Offer>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Offer?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<List<Offer>> GetByPostIdAsync(int postId)
        => await _repo.GetByPostIdAsync(postId);

    public async Task CreateAsync(Offer offer)
    {
        if (offer == null)
            throw new ArgumentNullException(nameof(offer));

        offer.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(offer);
        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var offer = await _repo.GetByIdAsync(id);

        if (offer == null)
            return;

        _repo.Delete(offer);
        await _repo.SaveAsync();
    }
    public async Task<List<Offer>> GetByUserIdAsync(int userId)
        =>  await _repo.GetByUserIdAsync(userId);
    public async Task UpdateAsync(Offer offer)
    {
        var existing = await _repo.GetByIdAsync(offer.Id);

        if (existing == null)
            return;

        existing.Price = offer.Price;
        existing.Message = offer.Message;

        await _repo.SaveAsync();
    }

    public async Task AcceptOfferAsync(int offerId, int userId)
    {
        var offer = await _repo.GetByIdAsync(offerId);

        if (offer == null)
            throw new Exception("Offer not found.");

        if (offer.Post.UserId != userId)
            throw new UnauthorizedAccessException("Only the post owner can accept offers.");

        if (offer.Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be accepted.");

        // Accept this offer
        offer.Status = OfferStatus.Accepted;

        // Close the post
        offer.Post.ClosedAt = DateTime.UtcNow;

        // Reject all other pending offers for this post
        var allOffers = await _repo.GetByPostIdAsync(offer.PostId);
        foreach (var other in allOffers.Where(o => o.Id != offerId && o.Status == OfferStatus.Pending))
        {
            other.Status = OfferStatus.Rejected;
        }

        await _repo.SaveAsync();
    }

    public async Task DeclineOfferAsync(int offerId, int userId)
    {
        var offer = await _repo.GetByIdAsync(offerId);

        if (offer == null)
            throw new Exception("Offer not found.");

        if (offer.Post.UserId != userId)
            throw new UnauthorizedAccessException("Only the post owner can decline offers.");

        if (offer.Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be declined.");

        offer.Status = OfferStatus.Rejected;

        await _repo.SaveAsync();
    }
}