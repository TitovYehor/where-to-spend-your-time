using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Comment;

/// <summary>
/// Represents a request to update an existing comment
/// </summary>
public class CommentUpdateRequest
{
    /// <summary>
    /// The updated content of the comment
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;
}