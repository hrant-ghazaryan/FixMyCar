namespace FixMyCar.Web.Models;

public class PostMedia
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public MediaType Type { get; set; }

    public int Order { get; set; }
    public bool IsMain { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}
