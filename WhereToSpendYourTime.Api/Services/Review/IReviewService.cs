using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Review;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetReviewsForItemAsync(int itemId);

    Task<PagedResult<ReviewDto>> GetPagedReviewsForItemAsync(int itemId, ReviewFilterRequest filter);

    Task<PagedResult<ReviewDto>> GetPagedReviewsForUserAsync(string userId, ReviewFilterRequest filter);

    Task<ReviewDto> GetMyReviewForItemAsync(string userId, int itemId);

    Task<ReviewDto> GetByIdAsync(int id);

    Task<ReviewDto> CreateReviewAsync(string userId, ReviewCreateRequest request);

    Task UpdateReviewAsync(int reviewId, string userId, ReviewUpdateRequest request);

    Task DeleteReviewAsync(int reviewId, ApplicationUser user);
}