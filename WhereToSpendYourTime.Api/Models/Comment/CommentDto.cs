namespace WhereToSpendYourTime.Api.Models.Comment;

/// <summary>
/// Data transfer object representing a comment
/// </summary>
public class CommentDto
{
    /// <summary>
    /// The unique identifier of the comment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The content of the comment
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The name of the comment author
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the user who created the comment
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The role of the comment author
    /// </summary>
    public string AuthorRole { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the comment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The identifier of the review the comment belongs to
    /// </summary>
    public int ReviewId { get; set; }
}