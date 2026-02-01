using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Items;
using WhereToSpendYourTime.Api.Exceptions.Reviews;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

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
        return await _db.Reviews
            .AsNoTracking()
            .Where(r => r.ItemId == itemId)
            .OrderByDescending(r => r.CreatedAt)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PagedResult<ReviewDto>> GetPagedReviewsForItemAsync(int itemId, ReviewFilterRequest filter)
    {
        var query = _db.Reviews
            .AsNoTracking()
            .Where(r => r.ItemId == itemId)
            .OrderByDescending(r => r.CreatedAt);

        return await query
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<PagedResult<ReviewDto>> GetPagedReviewsForUserAsync(string userId, ReviewFilterRequest filter)
    {
        var query = _db.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt);

        return await query
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<ReviewDto> GetMyReviewForItemAsync(string userId, int itemId)
    {
        return await _db.Reviews
            .AsNoTracking()
            .Where(r => r.ItemId == itemId && r.UserId == userId)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync()
            ?? throw new UserItemReviewNotFoundException(userId, itemId);
    }

    public async Task<ReviewDto> GetByIdAsync(int id)
    {
        return await _db.Reviews
            .AsNoTracking()
            .Where(r => r.Id == id)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync()
            ?? throw new ReviewNotFoundException(id);
    }

    public async Task<ReviewDto> CreateReviewAsync(string userId, ReviewCreateRequest request)
    {
        if (request.ItemId < 1)
        {
            throw new InvalidReviewException("Review itemId is invalid");
        }
        ValidateReview(request.Title, request.Content, request.Rating);

        var itemExists = await _db.Items.AnyAsync(i => i.Id == request.ItemId);
        if (!itemExists)
        {
            throw new ItemNotFoundException(request.ItemId);
        }

        var hasReviewed = await _db.Reviews
            .AnyAsync(r => r.ItemId == request.ItemId && r.UserId == userId);

        if (hasReviewed)
        {
            throw new ReviewAlreadyExistsException(request.ItemId);
        }

        var review = new Data.Entities.Review
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            Rating = request.Rating,
            ItemId = request.ItemId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return await _db.Reviews
            .AsNoTracking()
            .Where(c => c.Id == review.Id)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }

    public async Task UpdateReviewAsync(int reviewId, string userId, ReviewUpdateRequest request)
    {
        ValidateReview(request.Title, request.Content, request.Rating);

        var review = await _db.Reviews.SingleOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
        {
            throw new ReviewNotFoundException(reviewId);
        }
        if (review.UserId != userId)
        {
            throw new ReviewForbiddenException();
        }

        review.Title = request.Title.Trim();
        review.Content = request.Content.Trim();
        review.Rating = request.Rating;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int reviewId, ApplicationUser user)
    {
        var review = await _db.Reviews.FindAsync(reviewId) ?? throw new ReviewNotFoundException(reviewId);

        var isAdmin = await _db.UserRoles
            .AnyAsync(r => r.UserId == user.Id && r.Role.Name == "Admin");

        var isModerator = await _db.UserRoles
            .AnyAsync(r => r.UserId == user.Id && r.Role.Name == "Moderator");

        if (review.UserId != user.Id && !isAdmin && !isModerator)
        {
            throw new ReviewForbiddenException();
        }

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
    }

    private static void ValidateReview(string title, string content, int rating)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidReviewException("Review title cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidReviewException("Review content cannot be empty");
        }

        if (rating < 1 || rating > 5)
        {
            throw new InvalidReviewException("Review rating must be between 1 and 5");
        }
    }
}