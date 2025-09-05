using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Review;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ReviewService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsForItemAsync(int itemId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.User)
            .Where(r => r.ItemId == itemId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<PagedResult<ReviewDto>> GetPagedReviewsForItemAsync(int itemId, ReviewFilterRequest filter)
    {
        var query = _db.Reviews
            .AsNoTracking()
            .AsQueryable()
            .Include(r => r.User)
            .Where(r => r.ItemId == itemId);

        query = query.OrderByDescending(r => r.CreatedAt);

        var pagedResult = await query
            .Select(r => _mapper.Map<ReviewDto>(r))
            .ToPagedResultAsync(filter.Page, filter.PageSize);

        return pagedResult;
    }

    public async Task<ReviewDto> GetMyReviewForItemAsync(string userId, int itemId)
    {
        var review = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Item)
            .FirstOrDefaultAsync(r => r.ItemId == itemId && r.UserId == userId);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto?> GetByIdAsync(int id)
    {
        var review = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return null;
        }

        var dto = _mapper.Map<ReviewDto>(review);
        return dto;
    }

    public async Task<(bool Success, ReviewDto? Review, string? Error)> CreateReviewAsync(string userId, ReviewCreateRequest request)
    {
        bool hasReviewed = await _db.Reviews
            .AnyAsync(r => r.ItemId == request.ItemId && r.UserId == userId);

        if (hasReviewed)
        {
            return (false, null, "User already reviewed this item.");
        }

        var review = new Data.Entities.Review
        {
            Title = request.Title,
            Content = request.Content,
            Rating = request.Rating,
            ItemId = request.ItemId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return (true, _mapper.Map<ReviewDto>(review), null);
    }

    public async Task<bool> UpdateReviewAsync(int reviewId, string userId, ReviewUpdateRequest request)
    {
        var review = await _db.Reviews.FindAsync(reviewId);
        if (review == null || review.UserId != userId)
        {
            return false;
        }

        review.Title = request.Title;
        review.Content = request.Content;
        review.Rating = request.Rating;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin)
    {
        var review = await _db.Reviews.FindAsync(reviewId);
        if (review == null || (review.UserId != userId && !isAdmin))
        {
            return false;
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return true;
    }
}
