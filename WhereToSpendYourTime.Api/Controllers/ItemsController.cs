using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Item;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides item management and filtering operations
/// </summary>
/// <remarks>
/// Includes:
/// - Item filtering and pagination
/// - Item details
/// - Admin item management
/// - Item tag management
///
/// Base route: api/items
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        this._itemService = itemService;
    }

    /// <summary>
    /// Retrieves paginated items with optional filtering and sorting
    /// </summary>
    /// <param name="filter">Filtering, sorting and pagination parameters</param>
    /// <returns>Paginated list of items</returns>
    /// <response code="200">Items retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<ItemDto>>> GetItems([FromQuery] ItemFilterRequest filter)
    {
        var items = await _itemService.GetFilteredItemsAsync(filter);
        return Ok(items);
    }

    /// <summary>
    /// Retrieves an item by its identifier
    /// </summary>
    /// <param name="id">Item identifier</param>
    /// <returns>The requested item</returns>
    /// <response code="200">Item retrieved successfully</response>
    /// <response code="404">Item not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> GetItemById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);
        return Ok(item);
    }

    /// <summary>
    /// Adds a tag to an item
    /// </summary>
    /// <param name="id">Item identifier</param>
    /// <param name="tag">Tag name</param>
    /// <returns>The added tag</returns>
    /// <response code="200">Tag added successfully</response>
    /// <response code="400">Invalid tag name</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">Item not found</response>
    /// <response code="409">Tag already assigned to item</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/tags")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TagDto>> AddTagForItem(int id, [FromBody] TagRequest tag)
    {
        var result = await _itemService.AddTagForItemAsync(id, tag.Name);
        return Ok(result);
    }

    /// <summary>
    /// Removes a tag from an item
    /// </summary>
    /// <param name="id">Item identifier</param>
    /// <param name="tagName">Tag name</param>
    /// <returns>No content if the tag was removed from item successfully</returns>
    /// <response code="204">Tag removed successfully</response>
    /// <response code="400">Invalid tag name</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">Item or tag not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}/tags/remove/{tagName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagFromItem(int id, string tagName)
    {
        await _itemService.RemoveTagFromItemAsync(id, tagName);
        return NoContent();
    }

    /// <summary>
    /// Creates a new item
    /// </summary>
    /// <param name="request">Item creation data</param>
    /// <returns>The newly created item</returns>
    /// <response code="201">Item created successfully</response>
    /// <response code="400">Invalid item data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden (Admin role required)</response>
    /// <response code="404">Category not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequest request)
    {
        var item = await _itemService.CreateAsync(request);
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Updates an existing item
    /// </summary>
    /// <param name="id">Item identifier</param>
    /// <param name="request">Updated item data</param>
    /// <returns>No content if the item was updated successfully</returns>
    /// <response code="204">Item updated successfully</response>
    /// <response code="400">Invalid item data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden (Admin role required)</response>
    /// <response code="404">Item or category not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemUpdateRequest request)
    {
        await _itemService.UpdateAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes an item
    /// </summary>
    /// <param name="id">Item identifier</param>
    /// <returns>No content if the item was deleted successfully</returns>
    /// <response code="204">Item deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">Item not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _itemService.DeleteAsync(id);
        return NoContent();
    }
}