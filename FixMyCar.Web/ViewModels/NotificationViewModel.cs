namespace FixMyCar.Web.ViewModels;

public class NotificationViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RelatedUserName { get; set; } = string.Empty;
    public int? RelatedPostId { get; set; }
    public int? RelatedOfferId { get; set; }
}
