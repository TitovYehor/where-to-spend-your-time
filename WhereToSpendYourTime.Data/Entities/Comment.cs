namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a comment written for a review
/// </summary>
public class Comment
{
    public int Id { get; set; }

    /// <summary>
    /// Content of the comment
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the comment was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ReviewId { get; set; }
    public Review? Review { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}