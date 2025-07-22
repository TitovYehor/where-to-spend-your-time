using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public double AverageRating { get; set; }

    public List<TagDto> Tags { get; set; } = [];
}
