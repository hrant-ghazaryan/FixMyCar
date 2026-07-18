using System.ComponentModel.DataAnnotations;

namespace FixMyCar.Web.ViewModels;

public class OfferCreateViewModel
{
    [Required, Range(1, 100_000_000)]
    public decimal Price { get; set; }

    [StringLength(1_000)]
    public string? Message { get; set; }
}

public class OfferEditViewModel : OfferCreateViewModel
{
    [Required]
    public int Id { get; set; }
}
