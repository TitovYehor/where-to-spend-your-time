namespace WhereToSpendYourTime.Data.Entities;

public class Media
{
    public int Id { get; set; }
    public int ItemId { get; set; }

    public MediaType Type { get; set; }
    public string Url { get; set; } = string.Empty;

    public Item Item { get; set; } = null!;
}

public enum MediaType
{
    Image,
    Video
}