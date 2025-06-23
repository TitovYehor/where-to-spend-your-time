using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public readonly IMapper _mapper;

    public UsersController(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        this._db = db;
        this._userManager = userManager;
        this._mapper = mapper;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    { 
        return await this.GetProfileInternal(_userManager.GetUserId(User)!, true);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("User ID cannot be null or empty.");
        }

        var isSelf = _userManager.GetUserId(User) == id;
        return await this.GetProfileInternal(id, isSelf);
    }

    private async Task<IActionResult> GetProfileInternal(string userId, bool isSelf)
    {
        var user = await _db.Users
            .Include(u => u.Reviews)
                .ThenInclude(r => r.Item)
            .Include(u => u.Comments)
                .ThenInclude(c => c.Review)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound();

        var dto = _mapper.Map<ApplicationUserDto>(user);

        dto.Reviews = user.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                Author = user.DisplayName
            }).ToList();

        dto.Comments = user.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                Author = user.DisplayName,
                CreatedAt = c.CreatedAt
            }).ToList();

        if (!isSelf)
        {
            dto.Email = null;
        }

        return Ok(dto);
    }
}
