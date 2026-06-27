namespace FixMyCar.Web.Models;

public class Favorite
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
