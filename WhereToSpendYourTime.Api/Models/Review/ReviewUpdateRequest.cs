using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Review;

/// <summary>
/// Represents a request to update an existing review
/// </summary>
public class ReviewUpdateRequest
{
    /// <summary>
    /// The updated title of the review
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The updated content of the review
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The updated rating for the item
    /// </summary>
    [Required]
    public int Rating { get; set; }
}