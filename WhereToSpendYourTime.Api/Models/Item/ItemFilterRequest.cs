using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemFilterRequest
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public List<int> TagsIds { get; set; } = [];
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 10;
}
