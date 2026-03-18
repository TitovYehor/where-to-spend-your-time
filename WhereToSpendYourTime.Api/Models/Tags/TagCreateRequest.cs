using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Tags;

/// <summary>
/// Represents a request to create a new tag
/// </summary>
public class TagCreateRequest
{
    /// <summary>
    /// The name of the tag
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}