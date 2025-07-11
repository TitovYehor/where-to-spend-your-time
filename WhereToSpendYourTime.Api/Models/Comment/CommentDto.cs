namespace WhereToSpendYourTime.Api.Models.Comment;

public class CommentDto
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int ReviewId { get; set; }
}
