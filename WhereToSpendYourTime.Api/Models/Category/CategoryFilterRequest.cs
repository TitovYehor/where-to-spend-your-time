using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Category;

/// <summary>
/// Represents filtering and pagination parameters for retrieving categories
/// </summary>
public class CategoryFilterRequest
{
    /// <summary>
    /// Page number of the result set
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of categories returned per page
    /// </summary>
    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 5;

    /// <summary>
    /// Optional search query used to filter categories by name
    /// </summary>
    public string? Search { get; set; } = string.Empty;
}