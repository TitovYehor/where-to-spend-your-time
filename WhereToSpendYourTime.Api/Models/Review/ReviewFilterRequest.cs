using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Review;

/// <summary>
/// Represents pagination parameters for retrieving reviews
/// </summary>
public class ReviewFilterRequest
{
    /// <summary>
    /// The page number to retrieve
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of reviews to return per page
    /// </summary>
    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 5;
}