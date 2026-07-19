namespace FixMyCar.Web.Models;

using System.Text.Json.Serialization;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }
    
    [JsonIgnore]
    public User? User { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // 🔗 Related entities (nullable)
    public int? RelatedOfferId { get; set; }
    
    [JsonIgnore]
    public Offer? RelatedOffer { get; set; }

    public int? RelatedPostId { get; set; }
    
    [JsonIgnore]
    public Post? RelatedPost { get; set; }

    public int? RelatedUserId { get; set; } // Who triggered the notification
    
    [JsonIgnore]
    public User? RelatedUser { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}

public enum NotificationType
{
    OfferCreated = 1,
    OfferAccepted = 2,
    OfferRejected = 3,
    ReviewAdded = 4,
    PostClosed = 5,
    FavoriteAdded = 6
}
