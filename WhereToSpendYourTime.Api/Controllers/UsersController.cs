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

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = _userManager.GetUserId(User)!;
        var dto = await _userService.GetProfileAsync(userId, isSelf: true);

        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("User ID cannot be null or empty.");
        }

        var isSelf = _userManager.GetUserId(User) == id;
        var dto = await _userService.GetProfileAsync(id, isSelf);

        return dto == null ? NotFound() : Ok(dto);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length < 2)
        {
            return BadRequest("Display name must be at least 2 characters.");
        }

        var userId = _userManager.GetUserId(User)!;
        
        var success = await _userService.UpdateProfileAsync(userId, request.DisplayName);

        return success ? NoContent() : BadRequest("Failed to update profile");
    }

    [Authorize]
    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword.Length < 6)
        {
            return BadRequest("Password must be at least 6 characters.");
        }

        var userId = _userManager.GetUserId(User)!;
        
        var (succeeded, errors) = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        if (!succeeded)
        {
            return BadRequest(errors);
        }

        return succeeded ? NoContent() : BadRequest("Failed to update password");
    }
}
