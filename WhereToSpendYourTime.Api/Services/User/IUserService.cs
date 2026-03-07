using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.User;

namespace WhereToSpendYourTime.Api.Services.User;

/// <summary>
/// Provides operations for managing application users,
/// including profile management, role administration,
/// and credential updates
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves all registered users ordered by display name and role
    /// </summary>
    /// <returns>
    /// A collection of <see cref="ApplicationUserDto"/> representing all users
    /// </returns>
    Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();

    /// <summary>
    /// Retrieves a paginated list of users with optional
    /// role and search filtering
    /// </summary>
    /// <param name="filter">
    /// Pagination and filtering parameters
    /// </param>
    /// <returns>
    /// A paginated result of <see cref="ApplicationUserDto"/> containing users matching the filter criteria
    /// </returns>
    Task<PagedResult<ApplicationUserDto>> GetPagedUsersAsync(UserFilterRequest filter);

    /// <summary>
    /// Retrieves a user's profile information
    /// </summary>
    /// <param name="userId">The id of the requested user</param>
    /// <param name="isSelf">
    /// Indicates whether the requesting user is accessing their own profile.
    /// Sensitive data such as email is hidden when false
    /// </param>
    /// <returns>
    /// The user's <see cref="ApplicationUserDto"/> profile data
    /// </returns>
    /// <exception cref="UserNotFoundException">
    /// Thrown when the specified user does not exist
    /// </exception>
    Task<ApplicationUserDto> GetProfileAsync(string userId, bool isSelf);

    /// <summary>
    /// Retrieves all available application roles
    /// </summary>
    /// <returns>
    /// A collection containing role names
    /// </returns>
    Task<IEnumerable<string>> GetRolesAsync();

    /// <summary>
    /// Updates a user's display name
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="displayName">The new display name</param>
    /// <exception cref="InvalidUserDisplayNameException">
    /// Thrown when the display name is invalid
    /// </exception>
    /// <exception cref="UserNotFoundException">
    /// Thrown when the user does not exist
    /// </exception>
    /// <exception cref="DemoAccountOperationForbiddenException">
    /// Thrown when attempting to modify the demo account
    /// </exception>
    /// <exception cref="UserProfileUpdateFailedException">
    /// Thrown when the update operation fails
    /// </exception>
    Task UpdateProfileAsync(string userId, string displayName);

    /// <summary>
    /// Changes the password of a user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="currentPassword">The user's current password</param>
    /// <param name="newPassword">The new password</param>
    /// <exception cref="UserNotFoundException">
    /// Thrown when the user does not exist
    /// </exception>
    /// <exception cref="DemoAccountOperationForbiddenException">
    /// Thrown when attempting to modify the demo account
    /// </exception>
    /// <exception cref="PasswordChangeFailedException">
    /// Thrown when the password change fails
    /// </exception>
    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    /// <summary>
    /// Updates the role assigned to a user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="newRole">The role to assign</param>
    /// <exception cref="InvalidRoleException">
    /// Thrown when the provided role is invalid
    /// </exception>
    /// <exception cref="RoleNotFoundException">
    /// Thrown when the role does not exist
    /// </exception>
    /// <exception cref="UserNotFoundException">
    /// Thrown when the user does not exist
    /// </exception>
    /// <exception cref="UserRoleForbiddenException">
    /// Thrown when attempting to modify a protected user's role
    /// </exception>
    /// <exception cref="UserRoleUpdateFailedException">
    /// Thrown when role assignment fails
    /// </exception>
    Task UpdateUserRoleAsync(string userId, string newRole);

    /// <summary>
    /// Deletes a user account
    /// </summary>
    /// <param name="userId">The id of the user to delete</param>
    /// <exception cref="UserNotFoundException">
    /// Thrown when the user does not exist
    /// </exception>
    /// <exception cref="UserDeleteForbiddenException">
    /// Thrown when attempting to delete a protected user
    /// </exception>
    /// <exception cref="UserDeleteFailedException">
    /// Thrown when deletion fails
    /// </exception>
    Task DeleteUserAsync(string userId);
}