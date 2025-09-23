using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Review;

public class ReviewCreateRequest
{
    [Required]
    public int ItemId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int Rating { get; set; }
}
