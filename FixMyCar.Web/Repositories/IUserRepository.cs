using FixMyCar.Web.Models;

namespace FixMyCar.Web.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByUserNameAsync(string userName);

    Task AddAsync(User user);
    Task<User?> GetByPhoneAsync(string phoneNumber);
    Task SaveAsync();
    Task<IEnumerable<User>> GetAllAsync();
}
