namespace WhereToSpendYourTime.Api.Models.Review;

public class ReviewCreateRequest
{
    public int ItemId { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int Rating { get; set; }
}
