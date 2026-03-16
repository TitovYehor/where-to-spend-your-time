using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Item;

/// <summary>
/// Represents filtering and pagination parameters for retrieving items
/// </summary>
public class ItemFilterRequest
{
    /// <summary>
    /// Search text used to filter items by title or description
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filters items by category identifier
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filters items by tag identifiers
    /// </summary>
    public List<int> TagsIds { get; set; } = [];

    /// <summary>
    /// The field used to sort the results
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Indicates whether sorting should be in descending order
    /// </summary>
    public bool Descending { get; set; } = true;

    /// <summary>
    /// The page number to retrieve
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items to return per page
    /// </summary>
    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 10;
}