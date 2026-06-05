namespace FixMyCar.Web.Models;

public class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? City { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
}
