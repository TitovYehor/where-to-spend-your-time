using WhereToSpendYourTime.Api.Models.Auth;

namespace WhereToSpendYourTime.Api.Services.Auth;

/// <summary>
/// Defines authentication-related operations such as user registration,
/// login and logout
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">
    /// The registration request containing email, password and display name
    /// </param>
    /// <exception cref="InvalidRegisterRequestException">
    /// Thrown when the request is null or contains invalid data
    /// </exception>
    /// <exception cref="UserAlreadyExistsException">
    /// Thrown when a user with the provided email already exists
    /// </exception>
    /// <exception cref="RegisterFailedException">
    /// Thrown when user creation fails due to identity validation errors
    /// </exception>
    /// <exception cref="UserRoleAssignmentFailedException">
    /// Thrown when assigning the default role to the user fails
    /// </exception>
    Task RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates an existing user using email and password
    /// </summary>
    /// <param name="request">
    /// The login request containing user credentials
    /// </param>
    /// <exception cref="InvalidLoginRequestException">
    /// Thrown when the request is null or contains invalid data
    /// </exception>
    /// <exception cref="InvalidCredentialsException">
    /// Thrown when authentication fails due to invalid email or password
    /// </exception>
    Task LoginAsync(LoginRequest request);

    /// <summary>
    /// Signs out the currently authenticated user
    /// </summary>
    /// <remarks>
    /// Clears the authentication session or cookie
    /// </remarks>
    Task LogoutAsync();
}