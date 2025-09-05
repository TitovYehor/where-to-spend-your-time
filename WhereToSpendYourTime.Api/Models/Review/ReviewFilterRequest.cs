namespace WhereToSpendYourTime.Api.Models.Review;

public class ReviewFilterRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}