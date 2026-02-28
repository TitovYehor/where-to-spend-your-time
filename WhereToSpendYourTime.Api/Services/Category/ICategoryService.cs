using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;

namespace WhereToSpendYourTime.Api.Services.Category;

/// <summary>
/// Provides operations for managing categories and retrieving
/// category-related data
/// </summary>
/// <remarks>
/// This service encapsulates all category-related business logic,
/// including querying, filtering, pagination, creation, updating
/// and deletion
/// </remarks>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves all categories ordered alphabetically by name
    /// </summary>
    /// <returns>
    /// A collection of all categories sorted by name
    /// </returns>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    /// <summary>
    /// Retrieves a paginated list of categories based on the provided filter criteria
    /// </summary>
    /// <param name="filter">
    /// The filtering and pagination parameters including search term,
    /// page number and page size
    /// </param>
    /// <returns>
    /// A paged result containing categories that match the specified filter
    /// </returns>
    Task<PagedResult<CategoryDto>> GetPagedCategoriesAsync(CategoryFilterRequest filter);

    /// <summary>
    /// Retrieves a category by its id
    /// </summary>
    /// <param name="id">The category id</param>
    /// <returns>
    /// The category matching the specified id
    /// </returns>
    /// <exception cref="CategoryNotFoundException">
    /// Thrown when the category with the specified id does not exist
    /// </exception>
    Task<CategoryDto> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all items that belong to the specified category
    /// </summary>
    /// <param name="categoryId">The category id</param>
    /// <returns>
    /// A collection of items associated with the specified category
    /// </returns>
    Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">
    /// The request containing the category name
    /// </param>
    /// <returns>
    /// The newly created category
    /// </returns>
    /// <exception cref="InvalidCategoryException">
    /// Thrown when the category name is invalid
    /// </exception>
    /// <exception cref="CategoryAlreadyExistsException">
    /// Thrown when a category with the same name already exists
    /// </exception>
    Task<CategoryDto> CreateCategoryAsync(CategoryCreateRequest request);

    /// <summary>
    /// Updates the name of an existing category
    /// </summary>
    /// <param name="id">The category id</param>
    /// <param name="request">
    /// The request containing the updated category name
    /// </param>
    /// <exception cref="InvalidCategoryException">
    /// Thrown when the category name is invalid
    /// </exception>
    /// <exception cref="CategoryNotFoundException">
    /// Thrown when the category does not exist
    /// </exception>
    /// <exception cref="CategoryAlreadyExistsException">
    /// Thrown when another category with the same name already exists
    /// </exception>
    Task UpdateCategoryAsync(int id, CategoryUpdateRequest request);

    /// <summary>
    /// Deletes the specified category
    /// </summary>
    /// <param name="id">The category id</param>
    /// <exception cref="CategoryNotFoundException">
    /// Thrown when the category does not exist
    /// </exception>
    Task DeleteCategoryAsync(int id);
}