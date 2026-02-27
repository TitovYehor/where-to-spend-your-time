using Microsoft.AspNetCore.Identity;
using WhereToSpendYourTime.Api.Exceptions.Auth;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Auth;

/// <summary>
/// Provides authentication and user management functionality
/// using ASP.NET Core Identity
/// </summary>
/// <remarks>
/// This service is responsible for:
/// - User registration
/// - User authentication
/// - Default role assignment
/// - User logout
/// </remarks>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private const string DefaultRole = "User";

    public AuthService(UserManager<ApplicationUser> userManager,
                       SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <inheritdoc />
    public async Task RegisterAsync(RegisterRequest request)
    {
        if (request == null)
        {
            throw new InvalidRegisterRequestException("Register request cannot be null");
        }
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.DisplayName))
        {
            throw new InvalidRegisterRequestException("Email, password and display name are required");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new RegisterFailedException(result.Errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, DefaultRole);
        if (!roleResult.Succeeded)
        {
            throw new UserRoleAssignmentFailedException(user.Id, DefaultRole, roleResult.Errors);
        }
    }

    /// <inheritdoc />
    public async Task LoginAsync(LoginRequest request)
    {
        if (request == null)
        {
            throw new InvalidLoginRequestException("Login request cannot be null");
        }
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidLoginRequestException("Email and password are required");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            throw new InvalidCredentialsException();
        }
    }

    /// <inheritdoc />
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}