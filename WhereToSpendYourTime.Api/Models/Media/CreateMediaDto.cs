using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Api.Models.Media;

public class CreateMediaDto
{
    public int ItemId { get; set; }

    public MediaType Type { get; set; }
    
    public IFormFile File { get; set; } = null!;
}
