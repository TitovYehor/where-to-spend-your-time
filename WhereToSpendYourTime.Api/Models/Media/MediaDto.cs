using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Api.Models.Media;

public class MediaDto
{
    public int Id { get; set; }

    public MediaType Type { get; set; }
    
    public string Url { get; set; } = string.Empty;
}
