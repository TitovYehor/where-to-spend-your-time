using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Comment;

/// <summary>
/// Provides operations for retrieving and managing comments
/// associated with reviews and users
/// </summary>
/// <remarks>
/// This service encapsulates comment-related business rules,
/// including validation, ownership checks and role-based authorization
/// </remarks>
public interface ICommentService
{
    /// <summary>
    /// Retrieves all comments for a specific review ordered by creation date (newest first)
    /// </summary>
    /// <param name="reviewId">The review id</param>
    /// <returns>
    /// A list of comments associated with the specified review
    /// </returns>
    Task<List<CommentDto>> GetCommentsByReviewIdAsync(int reviewId);

    /// <summary>
    /// Retrieves a paginated list of comments for a specific review
    /// </summary>
    /// <param name="reviewId">The review id</param>
    /// <param name="filter">
    /// The pagination parameters including page number and page size
    /// </param>
    /// <returns>
    /// A paged result containing comments for the specified review
    /// </returns>
    Task<PagedResult<CommentDto>> GetPagedCommentsByReviewIdAsync(int reviewId, CommentFilterRequest filter);

    /// <summary>
    /// Retrieves a paginated list of comments created by a specific user
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="filter">
    /// The pagination parameters including page number and page size
    /// </param>
    /// <returns>
    /// A paged result containing comments created by the specified user
    /// </returns>
    Task<PagedResult<CommentDto>> GetPagedCommentsByUserIdAsync(string userId, CommentFilterRequest filter);

    /// <summary>
    /// Adds a new comment to a review
    /// </summary>
    /// <param name="reviewId">The review id</param>
    /// <param name="userId">The id of the user creating the comment</param>
    /// <param name="content">The comment content</param>
    /// <returns>
    /// The newly created comment
    /// </returns>
    /// <exception cref="InvalidCommentException">
    /// Thrown when the comment content is empty or invalid
    /// </exception>
    /// <exception cref="ReviewNotFoundException">
    /// Thrown when the specified review does not exist
    /// </exception>
    Task<CommentDto> AddCommentAsync(int reviewId, string userId, string content);

    /// <summary>
    /// Updates an existing comment
    /// </summary>
    /// <param name="commentId">The comment id</param>
    /// <param name="userId">
    /// The id of the user attempting to update the comment
    /// </param>
    /// <param name="newContent">The updated comment content</param>
    /// <exception cref="InvalidCommentException">
    /// Thrown when the new content is empty or invalid
    /// </exception>
    /// <exception cref="CommentNotFoundException">
    /// Thrown when the comment does not exist
    /// </exception>
    /// <exception cref="CommentForbiddenException">
    /// Thrown when the user is not authorized to update the comment
    /// </exception>
    Task UpdateCommentAsync(int commentId, string userId, string newContent);

    /// <summary>
    /// Deletes an existing comment
    /// </summary>
    /// <param name="commentId">The comment id</param>
    /// <param name="user">
    /// The currently authenticated user attempting to delete the comment
    /// </param>
    /// <exception cref="CommentNotFoundException">
    /// Thrown when the comment does not exist
    /// </exception>
    /// <exception cref="CommentForbiddenException">
    /// Thrown when the user is not authorized to delete the comment
    /// </exception>
    Task DeleteCommentAsync(int commentId, ApplicationUser user);
}