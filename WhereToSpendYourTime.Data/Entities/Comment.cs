namespace WhereToSpendYourTime.Data.Entities;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ReviewId { get; set; }
    public Review? Review { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
