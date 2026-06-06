using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;

namespace FixMyCar.Web.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
        => _userRepository = userRepository;

    public Task<IEnumerable<User>> GetAllAsync()
        => _userRepository.GetAllAsync();

    public async Task<User?> GetByEmailAsync(string email)
       => await _userRepository.GetByEmailAsync(email);

    public async Task<User?> GetByIdAsync(int id)
        => await _userRepository.GetByIdAsync(id);

    public async Task RegisterAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);

        if (existingUser != null)
            throw new Exception("Email already exists");

        var existingPhone = await _userRepository.GetByPhoneAsync(user.PhoneNumber);

        if (existingPhone != null)
            throw new Exception("Phone already exists");

        // password hashing
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveAsync();
    }

}
