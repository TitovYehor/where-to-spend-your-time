namespace WhereToSpendYourTime.Api.Models.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
    
    public string Role { get; set; } = string.Empty;
}
