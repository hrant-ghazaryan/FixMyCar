namespace FixMyCar.Web.Models;

public class Offer
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? EstimatedCompletionDays { get; set; }

    public string Currency { get; set; } = "AMD";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
