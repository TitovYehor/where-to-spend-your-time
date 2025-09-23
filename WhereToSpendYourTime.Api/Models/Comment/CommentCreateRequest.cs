using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Comment;

public class CommentCreateRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
