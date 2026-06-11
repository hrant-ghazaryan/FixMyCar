namespace FixMyCar.Web.ViewModels;

public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string BaseUrl { get; set; } = "/";
}
