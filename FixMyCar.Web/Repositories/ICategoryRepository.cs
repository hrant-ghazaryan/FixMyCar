using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category);
    Task SaveAsync();
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByAsync(int id);
}
