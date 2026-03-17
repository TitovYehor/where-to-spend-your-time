namespace WhereToSpendYourTime.Api.Models.Review;


/// <summary>
/// Data transfer object representing a review
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// The unique identifier of the review
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the review
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The content of the review
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The name of the review author
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the user who created the review
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The role of the review author
    /// </summary>
    public string AuthorRole { get; set; } = string.Empty;

    /// <summary>
    /// The title of the item being reviewed
    /// </summary>
    public string ItemTitle { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the reviewed item
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// The rating given to the item
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// The date and time when the review was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}