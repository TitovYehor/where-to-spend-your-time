using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Models.Item;

/// <summary>
/// Data transfer object representing an item
/// </summary>
public class ItemDto
{
    /// <summary>
    /// The unique identifier of the item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the item
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A description of the item
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the item's category
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// The name of the category the item belongs to
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// The average rating of the item based on user reviews
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// The list of tags assigned to the item
    /// </summary>
    public List<TagDto> Tags { get; set; } = [];

    /// <summary>
    /// The media files associated with the item
    /// </summary>
    public List<MediaDto> Media { get; set; } = [];
}