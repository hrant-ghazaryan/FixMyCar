using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(Category category)
        => await _context.Categories.AddAsync(category);

    public void Delete(Category category)
        =>  _context.Categories.Remove(category);

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.ToListAsync();

    public async Task<Category?> GetByAsync(int id)
        => await _context.Categories.FindAsync(id);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();

    public void Update(Category category)
        => _context.Categories.Update(category);
    public async Task<IEnumerable<Category>> GetAllChildrenAsync(int categoryId)
        => await _context.Categories.Where(c => c.ParentId == categoryId).ToListAsync();

    public async Task<IEnumerable<Category>> GetByParentId(int parentId)
        => await _context.Categories.Where(c => c.ParentId == parentId).ToListAsync();
}
