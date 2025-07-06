using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;

namespace WhereToSpendYourTime.Api.Models.User;

public class ApplicationUserDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Role { get; set; }

    public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}
