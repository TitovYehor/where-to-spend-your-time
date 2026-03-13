using System.ComponentModel.DataAnnotations;
using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Api.Models.Media;

/// <summary>
/// Represents a request to upload media for an item
/// </summary>
public class CreateMediaDto
{
    /// <summary>
    /// The identifier of the item the media belongs to
    /// </summary>
    [Required]
    public int ItemId { get; set; }

    /// <summary>
    /// The type of media being uploaded
    /// </summary>
    [Required]
    public MediaType Type { get; set; }

    /// <summary>
    /// The media file to upload
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;
}