using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Category;

public class CategoryCreateRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
