namespace WhereToSpendYourTime.Api.Models.Pagination;

public class PagedResult<T>
{
    public List<T> Items { get; }

    public int TotalCount { get; }

    public PagedResult(List<T> items, int totalCount)
    {
        this.Items = items;
        this.TotalCount = totalCount;
    }
}
