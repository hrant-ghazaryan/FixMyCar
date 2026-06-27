using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class FavoriteRepository(AppDbContext context) : IFavoriteRepository
{
    private readonly AppDbContext _context = context;
    public async Task AddAsync(Favorite favorite)
        => await _context.AddAsync(favorite);

    public async Task<Favorite?> GetAsync(int userId, int postId)
        => await _context.Favorites
            .SingleOrDefaultAsync(f =>
                f.UserId == userId &&
                f.PostId == postId);

    public async Task<List<Favorite>> GetByUserIdAsync(int userId)
         => await _context.Favorites
            .Include(f => f.Post)
                .ThenInclude(p => p!.Media)
            .Include(f => f.Post)
                .ThenInclude(p => p!.Category)
            .Where(f => f.UserId == userId)
            .ToListAsync();

    public async Task RemoveAsync(Favorite favorite)
        => _context.Remove(favorite);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
