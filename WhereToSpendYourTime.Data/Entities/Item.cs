namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents an item that users can review
/// </summary>
public class Item
{
    public int Id { get; set; }

    /// <summary>
    /// Title of the item
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the item
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Moderation status of the item
    /// </summary>
    public ItemStatus Status { get; set; } = ItemStatus.Pending;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    /// <summary>
    /// Reviews written for this item
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Tags assigned to this item
    /// </summary>
    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();

    /// <summary>
    /// Media files attached to the item
    /// </summary>
    public ICollection<Media> Media { get; set; } = new List<Media>();
}

/// <summary>
/// Moderation status of an item
/// </summary>
public enum ItemStatus
{
    Pending,
    Approved,
    Rejected
}