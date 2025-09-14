namespace WhereToSpendYourTime.Data.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
}
