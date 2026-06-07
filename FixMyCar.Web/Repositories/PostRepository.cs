using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
        => _context = context;

    public async Task<List<Post>> GetAllAsync()
        => await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Media)
            .ToListAsync();

    public async Task<Post?> GetByIdAsync(int id)
        => await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Offers)
            .Include(p => p.Media)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Post post)
        => await _context.Posts.AddAsync(post);

    public async Task DeleteAsync(Post post)
        => _context.Posts.Remove(post);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();

    public async Task<List<Post>> GetByUserIdAsync(int id)
        => await _context.Posts
        .Where(p => p.UserId == id)
        .Include(p => p.Category)
        .Include(p => p.User)
        .ToListAsync();

    public async Task UpdateAsync(Post model)
        =>  _context.Posts.Update(model);

    public async Task AddMediaAsync(PostMedia media)
        => await _context.PostMedia.AddAsync(media);
}