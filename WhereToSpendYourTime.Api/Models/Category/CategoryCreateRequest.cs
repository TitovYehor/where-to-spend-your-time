using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Category;

/// <summary>
/// Represents a request to create a new category
/// </summary>
public class CategoryCreateRequest
{
    /// <summary>
    /// Name of the category
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}