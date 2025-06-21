namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
}
