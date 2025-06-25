using WhereToSpendYourTime.Api.Models.Review;

namespace WhereToSpendYourTime.Api.Services.Review;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetReviewsForItemAsync(int itemId);
    
    Task<(bool Success, ReviewDto? Review, string? Error)> CreateReviewAsync(string userId, ReviewCreateRequest request);
    
    Task<bool> UpdateReviewAsync(int reviewId, string userId, ReviewUpdateRequest request);
    
    Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin);
}
