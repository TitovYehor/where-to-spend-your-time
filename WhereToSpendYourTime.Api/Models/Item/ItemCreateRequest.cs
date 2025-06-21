namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemCreateRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
}
