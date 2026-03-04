using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Review;

/// <summary>
/// Provides operations for managing reviews, including retrieval,
/// creation, updating, and deletion
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves all reviews associated with a specific item
    /// </summary>
    /// <param name="itemId">The id of the item</param>
    /// <returns>
    /// A collection of <see cref="ReviewDto"/> ordered by creation date
    /// </returns>
    Task<IEnumerable<ReviewDto>> GetReviewsForItemAsync(int itemId);

    /// <summary>
    /// Retrieves a paginated list of reviews for a specific item
    /// </summary>
    /// <param name="itemId">The id of the item</param>
    /// <param name="filter">The pagination filter parameters</param>
    /// <returns>
    /// A paginated result containing <see cref="ReviewDto"/> entries
    /// </returns>
    Task<PagedResult<ReviewDto>> GetPagedReviewsForItemAsync(int itemId, ReviewFilterRequest filter);

    /// <summary>
    /// Retrieves a paginated list of reviews created by a specific user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="filter">The pagination filter parameters</param>
    /// <returns>
    /// A paginated result containing <see cref="ReviewDto"/> entries
    /// </returns>
    Task<PagedResult<ReviewDto>> GetPagedReviewsForUserAsync(string userId, ReviewFilterRequest filter);

    /// <summary>
    /// Retrieves the review created by the specified user for a specific item
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="itemId">The id of the item</param>
    /// <returns>
    /// The <see cref="ReviewDto"/> representing the user's review
    /// </returns>
    /// <exception cref="UserItemReviewNotFoundException">
    /// Thrown when the user has not submitted a review for the specified item
    /// </exception>
    Task<ReviewDto> GetMyReviewForItemAsync(string userId, int itemId);

    /// <summary>
    /// Retrieves a review by its unique id
    /// </summary>
    /// <param name="id">The id of the review</param>
    /// <returns>
    /// The <see cref="ReviewDto"/> representing the review
    /// </returns>
    /// <exception cref="ReviewNotFoundException">
    /// Thrown when the review does not exist
    /// </exception>
    Task<ReviewDto> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new review for an item
    /// </summary>
    /// <param name="userId">The id of the user creating the review</param>
    /// <param name="request">The review creation payload</param>
    /// <returns>
    /// The created <see cref="ReviewDto"/>
    /// </returns>
    /// <exception cref="InvalidReviewException">
    /// Thrown when the review data is invalid
    /// </exception>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the associated item does not exist
    /// </exception>
    /// <exception cref="ReviewAlreadyExistsException">
    /// Thrown when the user has already reviewed the specified item
    /// </exception>
    Task<ReviewDto> CreateReviewAsync(string userId, ReviewCreateRequest request);

    /// <summary>
    /// Updates an existing review
    /// </summary>
    /// <param name="reviewId">The id of the review to update</param>
    /// <param name="userId">The id of the user performing the update</param>
    /// <param name="request">The updated review data</param>
    /// <exception cref="InvalidReviewException">
    /// Thrown when the review data is invalid
    /// </exception>
    /// <exception cref="ReviewNotFoundException">
    /// Thrown when the review does not exist
    /// </exception>
    /// <exception cref="ReviewForbiddenException">
    /// Thrown when the user is not authorized to update the review
    /// </exception>
    Task UpdateReviewAsync(int reviewId, string userId, ReviewUpdateRequest request);

    /// <summary>
    /// Deletes a review
    /// </summary>
    /// <param name="reviewId">The id of the review to delete</param>
    /// <param name="user">
    /// The user attempting to delete the review. Used to determine authorization
    /// </param>
    /// <exception cref="ReviewNotFoundException">
    /// Thrown when the review does not exist
    /// </exception>
    /// <exception cref="ReviewForbiddenException">
    /// Thrown when the user is not authorized to delete the review
    /// </exception>
    Task DeleteReviewAsync(int reviewId, ApplicationUser user);
}