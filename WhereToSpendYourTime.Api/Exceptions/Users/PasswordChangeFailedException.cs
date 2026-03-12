using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when a password change operation fails due to
/// validation or identity-related errors
/// </summary>
public sealed class PasswordChangeFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors describing
    /// why the password change operation failed
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public PasswordChangeFailedException(IEnumerable<IdentityError> errors)
        : base("Password change failed")
    {
        Errors = errors;
    }
}