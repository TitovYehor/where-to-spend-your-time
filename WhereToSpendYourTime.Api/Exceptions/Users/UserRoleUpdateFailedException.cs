using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when updating a user's role fails due to
/// validation or identity-related errors
/// </summary>
public sealed class UserRoleUpdateFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors describing
    /// why the role update operation failed
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public UserRoleUpdateFailedException(IEnumerable<IdentityError> errors)
        : base("User role change failed")
    {
        Errors = errors;
    }
}