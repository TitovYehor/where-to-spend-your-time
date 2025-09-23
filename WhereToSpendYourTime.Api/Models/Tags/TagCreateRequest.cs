using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Tags;

public class TagCreateRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
