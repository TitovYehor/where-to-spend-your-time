namespace WhereToSpendYourTime.Api.Models.Item;

public class PagedItemResult
{
    public List<ItemDto> Items { get; set; } = [];

    public int TotalCount { get; set; }
}
