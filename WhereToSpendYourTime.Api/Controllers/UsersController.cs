using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides user management and profile operations
/// </summary>
/// <remarks>
/// Includes:
/// - Admin user management
/// - Public user profiles
/// - Self profile management
/// - Role management
/// - Password changes
///
/// Base route: api/users
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
    {
        this._userService = userService;
        this._userManager = userManager;
    }

    /// <summary>
    /// Retrieves all users
    /// </summary>
    /// <returns>A collection of all registered users</returns>
    /// <response code="200">Users retrieved successfully</response>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves paginated users with optional filtering
    /// </summary>
    /// <returns>A paged result containing users</returns>
    /// <response code="200">Paged users retrieved successfully</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<ApplicationUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<ApplicationUserDto>>> GetPagedUsers([FromQuery] UserFilterRequest filter)
    {
        var pagedUsers = await _userService.GetPagedUsersAsync(filter);
        return Ok(pagedUsers);
    }

    /// <summary>
    /// Retrieves the authenticated user's profile
    /// </summary>
    /// <returns>The profile of the authenticated user</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationUserDto>> GetMyProfile()
    {
        var userId = _userManager.GetUserId(User)!;
        var dto = await _userService.GetProfileAsync(userId, isSelf: true);
        return Ok(dto);
    }

    /// <summary>
    /// Retrieves a user's public profile by ID
    /// </summary>
    /// <returns>The public profile of the specified user</returns>
    /// <param name="id">User identifier</param>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationUserDto>> GetProfile(string id)
    {
        var isSelf = _userManager.GetUserId(User) == id;
        var dto = await _userService.GetProfileAsync(id, isSelf);
        return Ok(dto);
    }

    /// <summary>
    /// Retrieves available system roles
    /// </summary>
    /// <returns>A collection of available role names</returns>
    /// <response code="200">Roles retrieved successfully</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetRoles()
    {
        var roles = await _userService.GetRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Updates the authenticated user's display name
    /// </summary>
    /// <returns>No content if the profile was updated successfully</returns>
    /// <response code="204">Profile updated successfully</response>
    /// <response code="400">Invalid display name</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = _userManager.GetUserId(User)!;
        await _userService.UpdateProfileAsync(userId, request.DisplayName);
        return NoContent();
    }

    /// <summary>
    /// Changes the authenticated user's password
    /// </summary>
    /// <returns>No content if the password was changed successfully</returns>
    /// <response code="204">Password changed successfully</response>
    /// <response code="400">Password change failed</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpPut("me/change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _userManager.GetUserId(User)!;
        await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        return NoContent();
    }

    /// <summary>
    /// Updates a user's role
    /// </summary>
    /// <returns>No content if the role was updated successfully</returns>
    /// <response code="204">User role updated successfully</response>
    /// <response code="400">Invalid role</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">User or role not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{userId}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] UpdateUserRoleRequest request)
    {
        await _userService.UpdateUserRoleAsync(userId, request.Role);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <returns>No content if the user was deleted successfully</returns>
    /// <response code="204">User deleted successfully</response>
    /// <response code="403">Deleting admin users is forbidden</response>
    /// <response code="404">User not found</response>
    /// <response code="400">User deletion failed</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        await _userService.DeleteUserAsync(userId);
        return NoContent();
    }
}