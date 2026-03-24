using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a media file associated with an item
/// </summary>
public class Media
{
    public int Id { get; set; }
    public int ItemId { get; set; }

    /// <summary>
    /// Type of the media file
    /// </summary>
    public MediaType Type { get; set; }

    /// <summary>
    /// URL pointing to the stored media file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    public Item Item { get; set; } = null!;
}