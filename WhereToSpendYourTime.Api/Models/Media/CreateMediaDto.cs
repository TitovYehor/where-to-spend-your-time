using System.ComponentModel.DataAnnotations;
using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Api.Models.Media;

public class CreateMediaDto
{
    [Required]
    public int ItemId { get; set; }

    [Required]
    public MediaType Type { get; set; }

    [Required]
    public IFormFile File { get; set; } = null!;
}
