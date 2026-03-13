using WhereToSpendYourTime.Api.Models.Review;

namespace WhereToSpendYourTime.Api.Models.Stats;

/// <summary>
/// Data transfer object representing aggregated statistics for the application
/// </summary>
public class StatsDto
{
    /// <summary>
    /// The list of items with the highest average ratings
    /// </summary>
    public List<ItemStatDto> TopRatedItems { get; set; } = new List<ItemStatDto>();

    /// <summary>
    /// The list of items with the highest number of reviews
    /// </summary>
    public List<ItemStatDto> MostReviewedItems { get; set; } = new List<ItemStatDto>();

    /// <summary>
    /// The users who have written the most reviews
    /// </summary>
    public List<UserStatDto> TopReviewers { get; set; } = new List<UserStatDto>();

    /// <summary>
    /// The most recently created reviews
    /// </summary>
    public List<ReviewDto> RecentReviews { get; set; } = new List<ReviewDto>();
}