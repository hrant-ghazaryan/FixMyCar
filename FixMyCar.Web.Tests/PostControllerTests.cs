using System.Security.Claims;
using FixMyCar.Web.Controllers;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xunit;

namespace FixMyCar.Web.Tests;

public class PostControllerTests
{
    [Fact]
    public async Task Edit_RejectsUserWhoDoesNotOwnThePost()
    {
        var posts = new FakePostService { Post = new Post { Id = 10, UserId = 1, CategoryId = 1, Title = "Post", Description = "Description" } };
        var controller = CreateController(posts);

        var result = await controller.Edit(new PostEditViewModel { Id = 10, CategoryId = 1, Title = "Changed", Description = "Changed" });

        Assert.IsType<ForbidResult>(result);
        Assert.False(posts.UpdateCalled);
    }

    [Fact]
    public async Task Create_RejectsUnsafeUpload()
    {
        var controller = CreateController(new FakePostService());
        var file = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "Files", "unsafe.exe")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        var result = await controller.Create(new PostCreateViewModel
        {
            Title = "Post",
            Description = "Description",
            CategoryId = 1,
            Files = [file]
        });

        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey("Files"));
    }

    private static PostController CreateController(FakePostService posts)
    {
        var controller = new PostController(posts, new FakeCategoryService(), new FakeUserService(), new FakeOfferService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "2")], "Test"))
                }
            }
        };

        return controller;
    }

    private sealed class FakePostService : IPostService
    {
        public Post? Post { get; init; }
        public bool UpdateCalled { get; private set; }
        public Task<List<Post>> GetByUserIdAsync(int id) => Task.FromResult(new List<Post>());
        public Task<List<Post>> GetAllAsync() => Task.FromResult(new List<Post>());
        public Task<Post?> GetByIdAsync(int id) => Task.FromResult(Post?.Id == id ? Post : null);
        public Task CreateAsync(Post post) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task UpdateAsync(Post model) { UpdateCalled = true; return Task.CompletedTask; }
        public Task AddMediaAsync(PostMedia media) => Task.CompletedTask;
        public Task IncrementViewCountAsync(int postId) => Task.CompletedTask;
        public Task<List<Post>> GetPagedAsync(int page, int pageSize, string? search) => Task.FromResult(new List<Post>());
        public Task<int> GetCountAsync(string? search) => Task.FromResult(0);
    }

    private sealed class FakeCategoryService : ICategoryService
    {
        public Task<List<Category>> GetAllAsync() => Task.FromResult(new List<Category>());
        public Task<Category?> GetByIdAsync(int id) => Task.FromResult<Category?>(null);
        public Task AddAsync(Category category) => Task.CompletedTask;
        public Task UpdateAsync(Category category) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task<List<Category>> GetForUserAsync() => Task.FromResult(new List<Category>());
        public Task<IEnumerable<Category>> GetByParentId(int parentId) => Task.FromResult<IEnumerable<Category>>(new List<Category>());
    }

    private sealed class FakeUserService : IUserService
    {
        public Task RegisterAsync(User user) => Task.CompletedTask;
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(null);
        public Task<User?> GetByIdAsync(int id) => Task.FromResult<User?>(null);
        public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>(new List<User>());
    }

    private sealed class FakeOfferService : IOfferService
    {
        public Task<List<Offer>> GetAllAsync() => Task.FromResult(new List<Offer>());
        public Task<Offer?> GetByIdAsync(int id) => Task.FromResult<Offer?>(null);
        public Task<List<Offer>> GetByPostIdAsync(int postId) => Task.FromResult(new List<Offer>());
        public Task CreateAsync(Offer offer) => Task.CompletedTask;
        public Task<List<Offer>> GetByUserIdAsync(int userId) => Task.FromResult(new List<Offer>());
        public Task DeleteAsync(int id, int userId) => Task.CompletedTask;
        public Task UpdateAsync(int id, int userId, decimal price, string? message) => Task.CompletedTask;
        public Task AcceptOfferAsync(int offerId, int userId) => Task.CompletedTask;
        public Task DeclineOfferAsync(int offerId, int userId) => Task.CompletedTask;
    }
}
