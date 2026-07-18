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

        if (offer.Price <= 0 || offer.Price > 100_000_000)
            throw new ArgumentOutOfRangeException(nameof(offer.Price));

        if (offer.Message.Length > 1_000)
            throw new ArgumentException("Offer message is too long.");

        offer.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(offer);
        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var offer = await _repo.GetByIdAsync(id);

        if (offer == null)
            throw new KeyNotFoundException("Offer not found.");

        if (offer.UserId != userId)
            throw new UnauthorizedAccessException("Only the offer author can delete it.");

        if (offer.Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be deleted.");

        _repo.Delete(offer);
        await _repo.SaveAsync();
    }
    public async Task<List<Offer>> GetByUserIdAsync(int userId)
        =>  await _repo.GetByUserIdAsync(userId);
    public async Task UpdateAsync(int id, int userId, decimal price, string? message)
    {
        var existing = await _repo.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException("Offer not found.");

        if (existing.UserId != userId)
            throw new UnauthorizedAccessException("Only the offer author can edit it.");

        if (existing.Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be edited.");

        if (price <= 0 || price > 100_000_000)
            throw new ArgumentOutOfRangeException(nameof(price));

        if (message?.Length > 1_000)
            throw new ArgumentException("Offer message is too long.");

        existing.Price = price;
        existing.Message = message?.Trim() ?? string.Empty;

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
