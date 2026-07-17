using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Services;

public class PostService(IPostRepository repo) : IPostService
{
    private readonly IPostRepository _repo = repo;

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
    {
        await _repo.UpdateAsync(model);
        await _repo.SaveAsync();
    }

    public async Task AddMediaAsync(PostMedia media)
    {
        await _repo.AddMediaAsync(media);
        await _repo.SaveAsync();
    }

    public async Task<int> GetCountAsync(string? search)
    {
        var posts = await _repo.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            posts = posts.Where(p =>
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        return posts.Count;
    }

    public async Task<List<Post>> GetPagedAsync(int page, int pageSize, string? search)
    {
        var posts = await _repo.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            posts = posts.Where(p =>
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        return posts
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public async Task IncrementViewCountAsync(int postId)
    {
        await _repo.IncrementViewCountAsync(postId);
        await _repo.SaveAsync();
    }
}