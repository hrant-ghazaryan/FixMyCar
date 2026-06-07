using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;

    public PostService(IPostRepository repo)
        => _repo = repo;

    public async Task<List<Post>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task CreateAsync(Post post)
    {
        if (string.IsNullOrWhiteSpace(post.Title))
            throw new Exception("Title is required");

        if (post.UserId <= 0)
            throw new Exception("User is required");

        if (post.CategoryId <= 0)
            throw new Exception("Category is required");

        post.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(post);
        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var post = await _repo.GetByIdAsync(id);

        if (post == null)
            throw new Exception("Post not found");

        await _repo.DeleteAsync(post);
        await _repo.SaveAsync();
    }

    public async Task<List<Post>> GetByUserIdAsync(int id)
        => await _repo.GetByUserIdAsync(id);

    public async Task UpdateAsync(Post model)
        => await _repo.UpdateAsync(model);

    public async Task AddMediaAsync(PostMedia media)
    {
        await _repo.AddMediaAsync(media);
        await _repo.SaveAsync();
    }
}