using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when user registration fails due to
/// validation or identity-related errors
/// </summary>
public sealed class RegisterFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors returned
    /// during the registration process
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public RegisterFailedException(IEnumerable<IdentityError> errors)
        : base("User registration failed")
    {
        Errors = errors;
    }
}