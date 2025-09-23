using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemCreateRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
}
