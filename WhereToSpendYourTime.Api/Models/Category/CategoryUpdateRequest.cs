using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Category;

public class CategoryUpdateRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
