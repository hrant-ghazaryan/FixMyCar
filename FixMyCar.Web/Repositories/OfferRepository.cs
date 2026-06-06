using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using Microsoft.EntityFrameworkCore;

public class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _context;

    public OfferRepository(AppDbContext context)
        => _context = context;

    public async Task<List<Offer>> GetAllAsync()
        => await _context.Offers.ToListAsync();

    public async Task<Offer?> GetByIdAsync(int id)
        => await _context.Offers.FindAsync(id);

    public async Task<List<Offer>> GetByPostIdAsync(int postId)
        =>  await _context.Offers
            .Where(o => o.PostId == postId)
            .ToListAsync();

    public async Task AddAsync(Offer offer)
        => await _context.Offers.AddAsync(offer);

    public void Delete(Offer offer)
        => _context.Offers.Remove(offer);

    // ⭐ NEW
    public async Task SaveAsync()
        => await _context.SaveChangesAsync();
    public async Task<List<Offer>> GetByUserIdAsync(int userId)
        => await _context.Offers
            .Where(o => o.UserId == userId)
            .Include(o => o.Post)
            .ToListAsync();
}