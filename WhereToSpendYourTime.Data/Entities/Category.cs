namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a category used to group items
/// </summary>
public class Category
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the category (must be unique)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Items belonging to this category
    /// </summary>
    public ICollection<Item> Items { get; set; } = new List<Item>();
}