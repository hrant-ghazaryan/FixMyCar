using System.ComponentModel.DataAnnotations;

namespace FixMyCar.Web.ViewModels;

public class PostEditViewModel
{
    [Required]
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2_000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string? City { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
