using Microsoft.AspNetCore.Mvc.Rendering;

namespace FixMyCar.Web.ViewModels;

public class PostCreateViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? City { get; set; }

    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public List<IFormFile>? Files { get; set; }
}