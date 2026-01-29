using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Services.Review;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

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

    [HttpGet("items/{itemId}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForItem(int itemId)
    {
        var reviews = await _reviewService.GetReviewsForItemAsync(itemId);
        return Ok(reviews);
    }

    [HttpGet("items/{itemId}/reviews/paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<ReviewDto>>> GetPagedReviewsForItem(int itemId, [FromQuery] ReviewFilterRequest filter)
    {
        var pagedReviews = await _reviewService.GetPagedReviewsForItemAsync(itemId, filter);
        return Ok(pagedReviews);
    }

    [HttpGet("users/{userId}/reviews/paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<ReviewDto>>> GetPagedReviewsForUser(string userId, [FromQuery] ReviewFilterRequest filter)
    {
        var pagedReviews = await _reviewService.GetPagedReviewsForUserAsync(userId, filter);
        return Ok(pagedReviews);
    }

    [Authorize]
    [HttpGet("items/{itemId}/reviews/my")]
    public async Task<ActionResult<ReviewDto>> GetMyReviewForItem(int itemId)
    {
        var user = await _userManager.GetUserAsync(User);
        var review = await _reviewService.GetMyReviewForItemAsync(user!.Id, itemId);
        return Ok(review);
    }

    [HttpGet("reviews/{id}")]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        return Ok(review);
    }

    [Authorize]
    [HttpPost("reviews")]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var review = await _reviewService.CreateReviewAsync(user!.Id, request);
        return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
    }

    [Authorize]
    [HttpPut("reviews/{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        await _reviewService.UpdateReviewAsync(id, user!.Id, request);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("reviews/{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        await _reviewService.DeleteReviewAsync(id, user!);
        return NoContent();
    }
}