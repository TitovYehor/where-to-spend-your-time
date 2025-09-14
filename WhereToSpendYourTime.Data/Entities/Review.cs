namespace WhereToSpendYourTime.Data.Entities;

public class Review
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int Rating { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
