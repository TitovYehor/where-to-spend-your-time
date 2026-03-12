using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when updating a user's profile fails due to
/// validation or identity-related errors
/// </summary>
public sealed class UserProfileUpdateFailedException : Exception
{
    /// <summary>
    /// Collection of identity errors describing
    /// why the profile update operation failed
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public UserProfileUpdateFailedException(IEnumerable<IdentityError> errors)
        : base("User profile update failed")
    {
        Errors = errors;
    }
}