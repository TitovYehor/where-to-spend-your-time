using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Tags;

/// <summary>
/// Represents a request to update an existing tag
/// </summary>
public class TagUpdateRequest
{
    /// <summary>
    /// The updated name of the tag
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}