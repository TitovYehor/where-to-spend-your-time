namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a review written by a user for an item
/// </summary>
public class Review
{
    public int Id { get; set; }

    /// <summary>
    /// Title of the review
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content of the review
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the review was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Rating given to the item
    /// </summary>
    public int Rating { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Comments attached to the review
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}