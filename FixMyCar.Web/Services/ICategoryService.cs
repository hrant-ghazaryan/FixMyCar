using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
    public Task<List<Category>> GetForUserAsync();
    public Task<IEnumerable<Category>> GetByParentId(int parentId);

}
