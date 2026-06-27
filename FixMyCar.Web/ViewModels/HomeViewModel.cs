using FixMyCar.Web.Models;

namespace FixMyCar.Web.ViewModels;

public class HomeViewModel
{
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public PaginationViewModel? Pagination { get; set; }
    public string? Search { get; set; }
    public List<int> FavoritePostIds { get; set; } = new();
}
