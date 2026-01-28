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
    public async Task<ActionResult<List<CommentDto?>>> GetComments(int reviewId)
    {
        var comments = await _commentService.GetCommentsByReviewIdAsync(reviewId);
        return Ok(comments);
    }

    [HttpGet("reviews/{reviewId}/comments/paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<CommentDto>>> GetPagedCommentsByReviewId(int reviewId, [FromQuery] CommentFilterRequest filter)
    {
        var comments = await _commentService.GetPagedCommentsByReviewIdAsync(reviewId, filter);
        return Ok(comments);
    }

    [HttpGet("users/{userId}/comments/paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<CommentDto>>> GetPagedCommentsByUserId(string userId, [FromQuery] CommentFilterRequest filter)
    {
        var comments = await _commentService.GetPagedCommentsByUserIdAsync(userId, filter);
        return Ok(comments);
    }

    [Authorize]
    [HttpPost("reviews/{reviewId}/comments")]
    public async Task<IActionResult> AddComment(int reviewId, [FromBody] CommentCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var commentDto = await _commentService.AddCommentAsync(reviewId, user!.Id, request.Content);
        return Ok(commentDto);
    }

    [Authorize]
    [HttpPut("comments/{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        await _commentService.UpdateCommentAsync(id, user!.Id, request.Content);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        await _commentService.DeleteCommentAsync(id, user!);
        return NoContent();
    }
}