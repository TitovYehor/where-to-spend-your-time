using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Services.Item;

/// <summary>
/// Provides operations for querying and managing items,
/// including filtering, sorting, tag management and CRUD operations
/// </summary>
/// <remarks>
/// This service encapsulates business rules related to:
/// - Advanced filtering and sorting
/// - Tag association management
/// - Category validation
/// - Item creation, update and deletion
/// </remarks>
public interface IItemService
{
    /// <summary>
    /// Retrieves a paginated list of items based on the provided filter criteria
    /// </summary>
    /// <param name="filter">
    /// Filtering and sorting parameters including search term,
    /// category, tags, sorting field and pagination settings
    /// </param>
    /// <returns>
    /// A paged result containing items that match the specified filter
    /// </returns>
    Task<PagedResult<ItemDto>> GetFilteredItemsAsync(ItemFilterRequest filter);

    /// <summary>
    /// Retrieves an item by its id
    /// </summary>
    /// <param name="id">The item id</param>
    /// <returns>
    /// The item matching the specified id
    /// </returns>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the item does not exist
    /// </exception>
    Task<ItemDto> GetByIdAsync(int id);

    /// <summary>
    /// Adds a tag to an existing item. If the tag does not exist,
    /// it will be created
    /// </summary>
    /// <param name="id">The item id</param>
    /// <param name="tagName">The name of the tag to add</param>
    /// <returns>
    /// The tag that was associated with the item
    /// </returns>
    /// <exception cref="InvalidTagException">
    /// Thrown when the tag name is empty or invalid
    /// </exception>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the item does not exist
    /// </exception>
    /// <exception cref="ItemTagAlreadyExistsException">
    /// Thrown when the item already contains the specified tag
    /// </exception>
    Task<TagDto> AddTagForItemAsync(int id, string tagName);

    /// <summary>
    /// Removes a tag from an existing item
    /// </summary>
    /// <param name="id">The item id</param>
    /// <param name="tagName">The name of the tag to remove</param>
    /// <exception cref="InvalidTagException">
    /// Thrown when the tag name is empty or invalid
    /// </exception>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the item does not exist
    /// </exception>
    /// <exception cref="ItemTagNotFoundException">
    /// Thrown when the tag is not associated with the item
    /// </exception>
    Task RemoveTagFromItemAsync(int id, string tagName);

    /// <summary>
    /// Creates a new item
    /// </summary>
    /// <param name="request">
    /// The request containing title, description and category id
    /// </param>
    /// <returns>
    /// The newly created item
    /// </returns>
    /// <exception cref="InvalidItemException">
    /// Thrown when the title or description is invalid
    /// </exception>
    /// <exception cref="CategoryNotFoundException">
    /// Thrown when the specified category does not exist
    /// </exception>
    Task<ItemDto> CreateAsync(ItemCreateRequest request);

    /// <summary>
    /// Updates an existing item
    /// </summary>
    /// <param name="id">The item id</param>
    /// <param name="request">
    /// The request containing updated title, description and category id
    /// </param>
    /// <exception cref="InvalidItemException">
    /// Thrown when the title or description is invalid
    /// </exception>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the item does not exist
    /// </exception>
    /// <exception cref="CategoryNotFoundException">
    /// Thrown when the specified category does not exist
    /// </exception>
    Task UpdateAsync(int id, ItemUpdateRequest request);

    /// <summary>
    /// Deletes an existing item
    /// </summary>
    /// <param name="id">The item id</param>
    /// <exception cref="ItemNotFoundException">
    /// Thrown when the item does not exist
    /// </exception>
    Task DeleteAsync(int id);
}