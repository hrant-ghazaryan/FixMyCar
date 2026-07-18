using System.ComponentModel.DataAnnotations;

namespace FixMyCar.Web.ViewModels;

public class RegisterViewModel
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(30)]
    public string? PhoneNumber { get; set; }
}

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
