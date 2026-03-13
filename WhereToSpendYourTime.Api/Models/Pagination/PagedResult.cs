namespace WhereToSpendYourTime.Api.Models.Pagination;

/// <summary>
/// Represents a paginated result returned from a query
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// The list of items for the current page
    /// </summary>
    public List<T> Items { get; }

    /// <summary>
    /// The total number of items available before pagination is applied
    /// </summary>
    public int TotalCount { get; }

    public PagedResult(List<T> items, int totalCount)
    {
        this.Items = items;
        this.TotalCount = totalCount;
    }
}