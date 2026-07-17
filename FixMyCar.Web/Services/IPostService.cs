using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface IPostService
{
    Task<List<Post>> GetByUserIdAsync(int id);
    Task<List<Post>> GetAllAsync();
    Task<Post?> GetByIdAsync(int id);
    Task CreateAsync(Post post);
    Task DeleteAsync(int id);
    Task UpdateAsync(Post model);
    Task AddMediaAsync(PostMedia media);
    Task IncrementViewCountAsync(int postId);
    Task<List<Post>> GetPagedAsync(int page, int pageSize, string? search);
    Task<int> GetCountAsync(string? search);
}