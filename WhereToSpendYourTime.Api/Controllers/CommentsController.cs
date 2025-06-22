using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public CommentsController(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _db = db;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet("reviews/{reviewId}/comments")]
    public async Task<IActionResult> GetComments(int reviewId)
    {
        var comments = await _db.Comments
            .Include(c => c.User)
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(_mapper.Map<List<CommentDto>>(comments));
    }

    [Authorize]
    [HttpPost("reviews/{reviewId}/comments")]
    public async Task<IActionResult> AddComment(int reviewId, CommentCreateRequest request)
    {
        var reviewExists = await _db.Reviews.AnyAsync(c => c.Id == reviewId);
        if (!reviewExists)
        {
            return BadRequest("Invalid ReviewId");
        }

        var user = await _userManager.GetUserAsync(User);

        var comment = new Comment
        {
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            ReviewId = reviewId,
            UserId = user!.Id
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<CommentDto>(comment));
    }

    [Authorize]
    [HttpPut("comments/{id}")]
    public async Task<IActionResult> UpdateComment(int id, CommentUpdateRequest request)
    {
        var comment = await _db.Comments.FindAsync(id);
        var user = await _userManager.GetUserAsync(User);

        if (comment == null)
        {
            return NotFound();
        }

        if (comment.UserId != user!.Id)
        {
            return Forbid();
        }

        comment.Content = request.Content;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        if (comment == null)
        {
            return NotFound();
        }

        if (comment.UserId != user!.Id && !isAdmin)
        {
            return Forbid();
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
