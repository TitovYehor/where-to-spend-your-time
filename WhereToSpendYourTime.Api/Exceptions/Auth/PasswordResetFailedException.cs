using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when user password reset fails due to
/// validation or identity-related errors
/// </summary>
public sealed class PasswordResetFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors returned
    /// during the password reset process
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public PasswordResetFailedException(IEnumerable<IdentityError> errors)
        : base("Password reset operation failed")
    {
        Errors = errors;
    }
}