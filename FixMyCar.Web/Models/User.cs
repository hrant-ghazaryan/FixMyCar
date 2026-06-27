namespace FixMyCar.Web.Models;

public class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<Favorite> Favorites { get; set; }
    = new List<Favorite>();
}