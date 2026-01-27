using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Comment;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsByReviewIdAsync(int reviewId);

    Task<PagedResult<CommentDto>> GetPagedCommentsByReviewIdAsync(int reviewId, CommentFilterRequest filter);

    Task<PagedResult<CommentDto>> GetPagedCommentsByUserIdAsync(string userId, CommentFilterRequest filter);

    Task<CommentDto> AddCommentAsync(int reviewId, string userId, string content);

    Task UpdateCommentAsync(int commentId, string userId, string newContent);

    Task DeleteCommentAsync(int commentId, ApplicationUser user);
}