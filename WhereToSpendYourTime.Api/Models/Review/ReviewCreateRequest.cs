using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Review;

/// <summary>
/// Represents a request to create a new review for an item
/// </summary>
public class ReviewCreateRequest
{
    /// <summary>
    /// The identifier of the item being reviewed
    /// </summary>
    [Required]
    public int ItemId { get; set; }

    /// <summary>
    /// The title of the review
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The content of the review
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The rating given to the item
    /// </summary>
    [Required]
    public int Rating { get; set; }
}