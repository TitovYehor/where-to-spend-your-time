using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Comment;

/// <summary>
/// Represents a request to create a new comment
/// </summary>
public class CommentCreateRequest
{
    /// <summary>
    /// The content of the comment
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;
}