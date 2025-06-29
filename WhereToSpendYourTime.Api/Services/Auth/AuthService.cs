using Microsoft.AspNetCore.Identity;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager,
                       SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return (false, result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        return (true, Enumerable.Empty<IdentityError>());
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return false;
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
        return result.Succeeded;
    }
}
