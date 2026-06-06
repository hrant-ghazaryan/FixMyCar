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
}