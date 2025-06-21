namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string CategoryName { get; set; } = string.Empty;
}
