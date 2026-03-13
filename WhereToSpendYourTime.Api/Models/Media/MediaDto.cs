using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Api.Models.Media;

/// <summary>
/// Data transfer object representing media associated with an item
/// </summary>
public class MediaDto
{
    /// <summary>
    /// The unique identifier of the media
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The type of the media
    /// </summary>
    public MediaType Type { get; set; }

    /// <summary>
    /// The URL where the media file is stored
    /// </summary>
    public string Url { get; set; } = string.Empty;
}