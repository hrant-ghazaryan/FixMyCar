using FixMyCar.Web.Models;

namespace FixMyCar.Web.Services;

public interface IUserService
{
    Task RegisterAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
}
