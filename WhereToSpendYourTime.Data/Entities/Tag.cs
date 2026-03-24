namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a tag used to classify items
/// </summary>
public class Tag
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the tag (must be unique)
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Items associated with this tag
    /// </summary>
    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
}