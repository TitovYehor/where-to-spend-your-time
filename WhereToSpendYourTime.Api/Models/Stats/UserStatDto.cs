namespace WhereToSpendYourTime.Api.Models.Stats;

public class UserStatDto
{
    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
    
    public int ReviewCount { get; set; }

    public string? Role { get; set; }
}
