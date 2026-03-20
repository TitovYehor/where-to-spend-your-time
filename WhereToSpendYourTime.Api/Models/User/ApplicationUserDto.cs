using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;

namespace WhereToSpendYourTime.Api.Models.User;

/// <summary>
/// Data transfer object representing an application user
/// </summary>
public class ApplicationUserDto
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the user
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The role assigned to the user
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// The list of reviews created by the user
    /// </summary>
    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

    /// <summary>
    /// The list of comments created by the user
    /// </summary>
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}