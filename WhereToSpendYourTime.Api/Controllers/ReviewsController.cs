using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Services.Review;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides operations for managing reviews
/// </summary>
/// <remarks>
/// Base route: api/reviews
/// </remarks>
[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
    {
        this._reviewService = reviewService;
        this._userManager = userManager;
    }

    /// <summary>
    /// Retrieves all reviews for a specific item
    /// </summary>
    /// <returns>A collection of reviews for the specified item</returns>
    /// <response code="200">Reviews retrieved successfully</response>
    [HttpGet("items/{itemId}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForItem(int itemId)
    {
        var reviews = await _reviewService.GetReviewsForItemAsync(itemId);
        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves paged reviews for a specific item
    /// </summary>
    /// <returns>A paged result containing reviews for the specified item</returns>
    /// <response code="200">Paged reviews retrieved successfully</response>
    [HttpGet("items/{itemId}/reviews/paged")]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<ReviewDto>>> GetPagedReviewsForItem(int itemId, [FromQuery] ReviewFilterRequest filter)
    {
        var pagedReviews = await _reviewService.GetPagedReviewsForItemAsync(itemId, filter);
        return Ok(pagedReviews);
    }

    /// <summary>
    /// Retrieves paged reviews created by a specific user
    /// </summary>
    /// <returns>A paged result containing the user's reviews</returns>
    /// <response code="200">Paged reviews retrieved successfully</response>
    [HttpGet("users/{userId}/reviews/paged")]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<ReviewDto>>> GetPagedReviewsForUser(string userId, [FromQuery] ReviewFilterRequest filter)
    {
        var pagedReviews = await _reviewService.GetPagedReviewsForUserAsync(userId, filter);
        return Ok(pagedReviews);
    }

    /// <summary>
    /// Retrieves the authenticated user's review for a specific item
    /// </summary>
    /// <returns>The review created by the authenticated user for the specified item</returns>
    /// <response code="200">Review retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Review not found</response>
    [Authorize]
    [HttpGet("items/{itemId}/reviews/my")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetMyReviewForItem(int itemId)
    {
        var user = await _userManager.GetUserAsync(User);
        var review = await _reviewService.GetMyReviewForItemAsync(user!.Id, itemId);
        return Ok(review);
    }

    /// <summary>
    /// Retrieves a review by its ID
    /// </summary>
    /// <returns>The review with the specified ID</returns>
    /// <response code="200">Review retrieved successfully</response>
    /// <response code="404">Review not found</response>
    [HttpGet("reviews/{id}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        return Ok(review);
    }

    /// <summary>
    /// Creates a new review for an item
    /// </summary>
    /// <returns>The newly created review</returns>
    /// <response code="201">Review created successfully</response>
    /// <response code="400">Invalid review data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Item not found</response>
    /// <response code="409">Review for item already exists</response>
    [Authorize]
    [HttpPost("reviews")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var review = await _reviewService.CreateReviewAsync(user!.Id, request);
        return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
    }

    /// <summary>
    /// Updates an existing review
    /// </summary>
    /// <returns>No content if the update was successful</returns>
    /// <response code="204">Review updated successfully</response>
    /// <response code="400">Invalid review data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not allowed to update this review</response>
    /// <response code="404">Review not found</response>
    [Authorize]
    [HttpPut("reviews/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        await _reviewService.UpdateReviewAsync(id, user!.Id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a review
    /// </summary>
    /// <returns>No content if deletion was successful</returns>
    /// <response code="204">Review deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User is not allowed to delete this review</response>
    /// <response code="404">Review not found</response>
    [Authorize]
    [HttpDelete("reviews/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        await _reviewService.DeleteReviewAsync(id, user!);
        return NoContent();
    }
}