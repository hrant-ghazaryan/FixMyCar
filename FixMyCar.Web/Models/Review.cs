using System;
using System.ComponentModel.DataAnnotations;

namespace FixMyCar.Web.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int ReviewerId { get; set; }
    public User Reviewer { get; set; } = null!;

    [Required]
    public int TargetUserId { get; set; }
    public User TargetUser { get; set; } = null!;

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
    public int Rating { get; set; }

    [Required]
    [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
