using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Tags;

/// <summary>
/// Represents a request containing tag information
/// </summary>
public class TagRequest
{
    /// <summary>
    /// The name of the tag
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}