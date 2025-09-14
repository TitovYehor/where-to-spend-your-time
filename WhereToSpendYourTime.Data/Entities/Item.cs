namespace WhereToSpendYourTime.Data.Entities;

public class Item
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ItemStatus Status { get; set; } = ItemStatus.Pending;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();

    public ICollection<Media> Media { get; set; } = new List<Media>();
}

public enum ItemStatus
{
    Pending,
    Approved,
    Rejected
}
