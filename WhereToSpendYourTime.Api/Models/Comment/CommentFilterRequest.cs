namespace WhereToSpendYourTime.Api.Models.Comment;

public class CommentFilterRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}
