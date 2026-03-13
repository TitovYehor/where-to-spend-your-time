namespace WhereToSpendYourTime.Api.Models.Stats;

/// <summary>
/// Data transfer object representing statistical information about an item
/// </summary>
public class ItemStatDto
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
    /// The name of the category the item belongs to
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The average rating of the item
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// The number of reviews the item has received
    /// </summary>
    public int ReviewCount { get; set; }
}