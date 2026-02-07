using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<ApplicationUserDto>>> GetPagedUsers([FromQuery] UserFilterRequest filter)
    {
        var pagedUsers = await _userService.GetPagedUsersAsync(filter);
        return Ok(pagedUsers);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApplicationUserDto>> GetMyProfile()
    {
        var userId = _userManager.GetUserId(User)!;
        var dto = await _userService.GetProfileAsync(userId, isSelf: true);
        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUserDto>> GetProfile(string id)
    {
        var isSelf = _userManager.GetUserId(User) == id;
        var dto = await _userService.GetProfileAsync(id, isSelf);
        return Ok(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetRoles()
    {
        var roles = await _userService.GetRolesAsync();
        return Ok(roles);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = _userManager.GetUserId(User)!;
        await _userService.UpdateProfileAsync(userId, request.DisplayName);
        return NoContent();
    }

    [Authorize]
    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _userManager.GetUserId(User)!;
        await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] UpdateUserRoleRequest request)
    {
        await _userService.UpdateUserRoleAsync(userId, request.Role);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        await _userService.DeleteUserAsync(userId);
        return NoContent();
    }
}