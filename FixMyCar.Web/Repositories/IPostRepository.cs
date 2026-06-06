using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface IPostRepository
{
    Task<List<Post>> GetAllAsync();
    Task<List<Post>> GetByUserIdAsync(int id);
    Task<Post?> GetByIdAsync(int id);
    Task AddAsync(Post post);
    Task DeleteAsync(Post post);
    Task SaveAsync();
    Task UpdateAsync(Post model);
}