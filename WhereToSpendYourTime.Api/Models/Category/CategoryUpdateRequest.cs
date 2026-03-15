using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Category;

/// <summary>
/// Represents a request to update an existing category
/// </summary>
public class CategoryUpdateRequest
{
    /// <summary>
    /// Updated category name
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
}