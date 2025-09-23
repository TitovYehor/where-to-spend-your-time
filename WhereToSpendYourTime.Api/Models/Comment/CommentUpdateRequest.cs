using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Comment;

public class CommentUpdateRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
