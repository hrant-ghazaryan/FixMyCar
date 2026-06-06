using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Offers)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Offers)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
    }

    public async Task DeleteAsync(Post post)
    {
        _context.Posts.Remove(post);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}