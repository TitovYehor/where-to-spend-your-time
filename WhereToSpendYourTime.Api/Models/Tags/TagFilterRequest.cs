using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Tags;

/// <summary>
/// Represents filtering and pagination parameters for retrieving tags
/// </summary>
public class TagFilterRequest
{
    /// <summary>
    /// The page number to retrieve
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of tags to return per page
    /// </summary>
    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 5;

    /// <summary>
    /// Optional search text used to filter tags by name
    /// </summary>
    public string? Search { get; set; } = string.Empty;
}