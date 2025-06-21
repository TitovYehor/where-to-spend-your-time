using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(AppDbContext db, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet("/api/items/{itemId}/reviews")]
    public async Task<IActionResult> GetReviewsForItem(int itemId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.User)
            .Where(r => r.ItemId == itemId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview(ReviewCreateRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var review = new Review
        {
            Title = request.Title,
            Content = request.Content,
            ItemId = request.ItemId,
            UserId = user!.Id,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<ReviewDto>(review));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, ReviewUpdateRequest request)
    {
        var review = await _db.Reviews.FindAsync(id);
        var user = await _userManager.GetUserAsync(User);

        if (review == null)
        {
            return NotFound();
        }

        if (review.UserId != user!.Id)
        {
            return Forbid();
        }

        review.Title = request.Title;
        review.Content = request.Content;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

        if (review == null)
        {
            return NotFound();
        }

        if (review.UserId != user!.Id && !isAdmin)
        {
            return Forbid();
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
