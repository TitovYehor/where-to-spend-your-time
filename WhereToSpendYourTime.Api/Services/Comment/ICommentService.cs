using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;

namespace WhereToSpendYourTime.Api.Services.Comment;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsByReviewIdAsync(int reviewId);

    Task<PagedResult<CommentDto>> GetPagedCommentsByReviewIdAsync(int reviewId, CommentFilterRequest filter);

    Task<CommentDto?> AddCommentAsync(int reviewId, string userId, string content);

    Task<bool> UpdateCommentAsync(int commentId, string userId, string newContent);

    Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin);
}
