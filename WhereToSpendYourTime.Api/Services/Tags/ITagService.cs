using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Services.Tags;

/// <summary>
/// Provides operations for managing tags, including retrieval,
/// creation, updating, and deletion
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Retrieves all tags ordered alphabetically by name
    /// </summary>
    /// <returns>
    /// A collection of <see cref="TagDto"/> representing all available tags
    /// </returns>
    Task<IEnumerable<TagDto>> GetTagsAsync();

    /// <summary>
    /// Retrieves a paginated list of tags with optional search filtering
    /// </summary>
    /// <param name="filter">The pagination and search filter parameters</param>
    /// <returns>
    /// A paginated result containing <see cref="TagDto"/> entries
    /// </returns>
    Task<PagedResult<TagDto>> GetPagedTagsAsync(TagFilterRequest filter);

    /// <summary>
    /// Retrieves a tag by its unique id
    /// </summary>
    /// <param name="id">The id of the tag</param>
    /// <returns>
    /// The <see cref="TagDto"/> representing the tag
    /// </returns>
    /// <exception cref="TagNotFoundException">
    /// Thrown when the tag does not exist
    /// </exception>
    Task<TagDto> GetTagByIdAsync(int id);

    /// <summary>
    /// Creates a new tag
    /// </summary>
    /// <param name="request">The tag creation payload</param>
    /// <returns>
    /// The created <see cref="TagDto"/>
    /// </returns>
    /// <exception cref="InvalidTagException">
    /// Thrown when the tag name is empty
    /// </exception>
    /// <exception cref="TagAlreadyExistsException">
    /// Thrown when a tag with the same name already exists
    /// </exception>
    Task<TagDto> CreateTagAsync(TagCreateRequest request);

    /// <summary>
    /// Updates the name of an existing tag
    /// </summary>
    /// <param name="id">The id of the tag to update</param>
    /// <param name="request">The updated tag data</param>
    /// <exception cref="InvalidTagException">
    /// Thrown when the tag name is empty
    /// </exception>
    /// <exception cref="TagNotFoundException">
    /// Thrown when the tag does not exist
    /// </exception>
    Task UpdateTagAsync(int id, TagUpdateRequest request);

    /// <summary>
    /// Deletes an existing tag
    /// </summary>
    /// <param name="id">The id of the tag to delete</param>
    /// <exception cref="TagNotFoundException">
    /// Thrown when the tag does not exist
    /// </exception>
    Task DeleteTagAsync(int id);
}