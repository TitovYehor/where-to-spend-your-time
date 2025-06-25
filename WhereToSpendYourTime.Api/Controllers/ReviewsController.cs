using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Services.Review;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
    {
        this._reviewService = reviewService;
        this._userManager = userManager;
    }

    [HttpGet("/api/items/{itemId}/reviews")]
    public async Task<IActionResult> GetReviewsForItem(int itemId)
    {
        var reviews = await _reviewService.GetReviewsForItemAsync(itemId);

        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview(ReviewCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        var (success, reviewDto, error) = await _reviewService.CreateReviewAsync(user!.Id, request);
        if (!success)
        {
            return BadRequest(error);
        }

        return Ok(reviewDto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, ReviewUpdateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var success = await _reviewService.UpdateReviewAsync(id, user!.Id, request);

        return success ? NoContent() : Forbid();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        var success = await _reviewService.DeleteReviewAsync(id, user!.Id, isAdmin);
        return success ? NoContent() : Forbid();
    }
}
