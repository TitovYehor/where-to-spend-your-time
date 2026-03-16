using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Item;

/// <summary>
/// Represents a request to update an existing item
/// </summary>
public class ItemUpdateRequest
{
    /// <summary>
    /// The updated title of the item
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The updated description of the item
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the category the item belongs to
    /// </summary>
    [Required]
    public int CategoryId { get; set; }
}