using Microsoft.AspNetCore.Mvc.Rendering;

namespace FixMyCar.Web.ViewModels;

public class CategoryCreateViewModel
{
    public string Name { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public List<SelectListItem> ParentCategories { get; set; } = new();
}
