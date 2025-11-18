using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Services.Comment;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentsController(ICommentService commentService, UserManager<ApplicationUser> userManager)
    {
        this._commentService = commentService;
        this._userManager = userManager;
    }

    [HttpGet("reviews/{reviewId}/comments")]
    public async Task<IActionResult> GetComments(int reviewId)
    {
        var comments = await _commentService.GetCommentsByReviewIdAsync(reviewId);

        return Ok(comments);
    }

    [HttpGet("reviews/{reviewId}/comments/paged")]
    public async Task<IActionResult> GetPagedCommentsByReviewId(int reviewId, [FromQuery] CommentFilterRequest filter)
    { 
        var comments = await _commentService.GetPagedCommentsByReviewIdAsync(reviewId, filter);

        return Ok(comments);
    }

    [HttpGet("users/{userId}/comments/paged")]
    public async Task<IActionResult> GetPagedCommentsByUserId(string userId, [FromQuery] CommentFilterRequest filter)
    {
        var comments = await _commentService.GetPagedCommentsByUserIdAsync(userId, filter);

        return Ok(comments);
    }

    [Authorize]
    [HttpPost("reviews/{reviewId}/comments")]
    public async Task<IActionResult> AddComment(int reviewId, CommentCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var commentDto = await _commentService.AddCommentAsync(reviewId, user!.Id, request.Content);

        if (commentDto == null)
        {
            return BadRequest("Invalid ReviewId");
        }

        return Ok(commentDto);
    }

    [Authorize]
    [HttpPut("comments/{id}")]
    public async Task<IActionResult> UpdateComment(int id, CommentUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var success = await _commentService.UpdateCommentAsync(id, user!.Id, request.Content);

        return success ? NoContent() : Forbid();
    }

    [Authorize]
    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");
        var isModerator = await _userManager.IsInRoleAsync(user!, "Moderator");

        var success = await _commentService.DeleteCommentAsync(id, user!.Id, isAdmin || isModerator);

        return success ? NoContent() : Forbid();
    }
}
