// Services/CategoryService.cs
using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;

namespace FixMyCar.Web.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
        => _repo = repo;

    public async Task<List<Category>> GetAllAsync()
        => await _repo.GetAllAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _repo.GetByAsync(id);

    public async Task AddAsync(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new Exception("Category name cannot be empty");

        await _repo.AddAsync(category);
        await _repo.SaveAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        var existing = await _repo.GetByAsync(category.Id);
        if (existing == null)
            throw new Exception("Category not found");

        existing.Name = category.Name;
        existing.ParentId = category.ParentId;

        await _repo.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repo.GetByAsync(id);
        if (category == null)
            throw new Exception("Category not found");

        _repo.Delete(category);
        await _repo.SaveAsync();
    }
    public async Task<List<Category>> GetForUserAsync()
    {
        var categories = await _repo.GetAllAsync();

        return categories
            .Where(c => c.ParentId == null)
            .ToList();
    }
}
