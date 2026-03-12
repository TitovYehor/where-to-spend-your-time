using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when deleting a user fails due to
/// validation or identity-related errors
/// </summary>
public sealed class UserDeleteFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors describing
    /// why the user deletion operation failed
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public UserDeleteFailedException(IEnumerable<IdentityError> errors)
        : base("User delete failed")
    {
        Errors = errors;
    }
}