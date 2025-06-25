using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data;
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
}
