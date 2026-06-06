using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCar.Web.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
        => _context = context;
    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    //SingleOrDefaultAsync after - UNIQUE Email
    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u =>  u.Email == email);

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByUserNameAsync(string userName)
        => await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();
    public async Task<User?> GetByPhoneAsync(string phoneNumber)
    => await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.ToListAsync();
}
