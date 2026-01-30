using WhereToSpendYourTime.Api.Models.Auth;

namespace WhereToSpendYourTime.Api.Services.Auth;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);

    Task LoginAsync(LoginRequest request);

    Task LogoutAsync();
}