using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when assigning a role to a user fails.
/// Contains identity errors returned by the underlying
/// identity management system
/// </summary>
public sealed class UserRoleAssignmentFailedException : Exception
{
    /// <summary>
    /// Identifier of the user for whom role assignment failed
    /// </summary>
    public string UserId { get; }

    /// <summary>
    /// Role that was attempted to be assigned
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Collection of identity errors describing
    /// why the operation failed
    /// </summary>
    public IEnumerable<IdentityError> Errors { get; }

    public UserRoleAssignmentFailedException(
        string userId,
        string role,
        IEnumerable<IdentityError> errors)
        : base($"Failed to assign role '{role}' to user '{userId}'")
    {
        UserId = userId;
        Role = role;
        Errors = errors;
    }
}