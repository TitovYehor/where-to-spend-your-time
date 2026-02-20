using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Services.Comment;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides comment management operations
/// </summary>
/// <remarks>
/// Includes:
/// - Review comment listing
/// - User comment listing
/// - Comment creation
/// - Comment editing (owner only)
/// - Comment deletion (owner, Admin, or Moderator)
///
/// Base route: api
/// </remarks>
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

    /// <summary>
    /// Retrieves all comments for a specific review
    /// </summary>
    /// <param name="reviewId">Review identifier</param>
    /// <returns>List of comments ordered by newest first</returns>
    /// <response code="200">Comments retrieved successfully</response>
    [HttpGet("reviews/{reviewId:int}/comments")]
    [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CommentDto?>>> GetComments(int reviewId)
    {
        var comments = await _commentService.GetCommentsByReviewIdAsync(reviewId);
        return Ok(comments);
    }

    /// <summary>
    /// Retrieves paginated comments for a specific review
    /// </summary>
    /// <param name="reviewId">Review identifier</param>
    /// <param name="filter">Pagination parameters</param>
    /// <returns>Paginated list of comments</returns>
    /// <response code="200">Paged comments retrieved successfully</response>
    [HttpGet("reviews/{reviewId:int}/comments/paged")]
    [ProducesResponseType(typeof(PagedResult<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<CommentDto>>> GetPagedCommentsByReviewId(int reviewId, [FromQuery] CommentFilterRequest filter)
    {
        var comments = await _commentService.GetPagedCommentsByReviewIdAsync(reviewId, filter);
        return Ok(comments);
    }

    /// <summary>
    /// Retrieves paginated comments created by a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="filter">Pagination parameters</param>
    /// <returns>Paginated list of user comments</returns>
    /// <response code="200">Paged comments retrieved successfully</response>
    [HttpGet("users/{userId}/comments/paged")]
    [ProducesResponseType(typeof(PagedResult<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<CommentDto>>> GetPagedCommentsByUserId(string userId, [FromQuery] CommentFilterRequest filter)
    {
        var comments = await _commentService.GetPagedCommentsByUserIdAsync(userId, filter);
        return Ok(comments);
    }

    /// <summary>
    /// Adds a new comment to a review
    /// </summary>
    /// <param name="reviewId">Review identifier</param>
    /// <param name="request">Comment content</param>
    /// <returns>The created comment</returns>
    /// <response code="200">Comment created successfully</response>
    /// <response code="400">Invalid comment content</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Review not found</response>
    [Authorize]
    [HttpPost("reviews/{reviewId:int}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(int reviewId, [FromBody] CommentCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var commentDto = await _commentService.AddCommentAsync(reviewId, user!.Id, request.Content);
        return Ok(commentDto);
    }

    /// <summary>
    /// Updates an existing comment
    /// </summary>
    /// <param name="id">Comment identifier</param>
    /// <param name="request">Updated comment content</param>
    /// <returns>No content if the comment was updated successfully</returns>
    /// <response code="204">Comment updated successfully</response>
    /// <response code="400">Invalid comment content</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not the comment owner</response>
    /// <response code="404">Comment not found</response>
    [Authorize]
    [HttpPut("comments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        await _commentService.UpdateCommentAsync(id, user!.Id, request.Content);
        return NoContent();
    }

    /// <summary>
    /// Deletes a comment
    /// </summary>
    /// <param name="id">Comment identifier</param>
    /// <returns>No content if the comment was deleted successfully</returns>
    /// <response code="204">Comment deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403"> User is not the comment owner, Admin, or Moderator </response>
    /// <response code="404">Comment not found</response>
    [Authorize]
    [HttpDelete("comments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        await _commentService.DeleteCommentAsync(id, user!);
        return NoContent();
    }
}