using WhereToSpendYourTime.Api.Models.Review;

namespace WhereToSpendYourTime.Api.Models.Stats;

public class StatsDto
{
    public List<ItemStatDto> TopRatedItems { get; set; } = new List<ItemStatDto>();

    public List<ItemStatDto> MostReviewedItems { get; set; } = new List<ItemStatDto>();

    public List<UserStatDto> TopReviewers { get; set; } = new List<UserStatDto>();

    public List<ReviewDto> RecentReviews { get; set; } = new List<ReviewDto>();
}
