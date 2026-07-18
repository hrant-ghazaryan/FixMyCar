using FixMyCar.Web.Models;
using FixMyCar.Web.Repositories;
using FixMyCar.Web.Services;
using Xunit;

namespace FixMyCar.Web.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_HashesPassword_AndNormalizesEmail()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);
        var user = new User { Email = " TEST@EXAMPLE.COM ", UserName = "Test", DisplayName = "Test", PasswordHash = "password123" };

        await service.RegisterAsync(user);

        Assert.Equal("test@example.com", user.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify("password123", user.PasswordHash));
        Assert.True(repository.SaveCalled);
    }

    [Fact]
    public async Task RegisterAsync_AllowsMultipleUsersWithoutPhoneNumber()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);

        await service.RegisterAsync(new User { Email = "one@example.com", UserName = "one", DisplayName = "One", PasswordHash = "password123" });
        await service.RegisterAsync(new User { Email = "two@example.com", UserName = "two", DisplayName = "Two", PasswordHash = "password123" });

        Assert.Equal(2, repository.Users.Count);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];
        public bool SaveCalled { get; private set; }
        public Task<User?> GetByIdAsync(int id) => Task.FromResult(Users.SingleOrDefault(x => x.Id == id));
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult(Users.SingleOrDefault(x => x.Email == email));
        public Task<User?> GetByUserNameAsync(string userName) => Task.FromResult(Users.SingleOrDefault(x => x.UserName == userName));
        public Task AddAsync(User user) { Users.Add(user); return Task.CompletedTask; }
        public Task<User?> GetByPhoneAsync(string? phoneNumber) => Task.FromResult(Users.SingleOrDefault(x => x.PhoneNumber == phoneNumber));
        public Task SaveAsync() { SaveCalled = true; return Task.CompletedTask; }
        public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>(Users);
    }
}
