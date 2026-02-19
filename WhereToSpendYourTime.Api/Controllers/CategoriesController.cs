using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Services.Category;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides category management operations
/// </summary>
/// <remarks>
/// Includes:
/// - Public category listing
/// - Paginated filtering
/// - Category details
/// - Category item retrieval
/// - Admin category management
///
/// Base route: api/categories
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    /// <summary>
    /// Retrieves all categories
    /// </summary>
    /// <returns>List of categories ordered by name</returns>
    /// <response code="200">Categories retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves paginated categories with optional search filtering
    /// </summary>
    /// <param name="filter">Pagination and search filter parameters</param>
    /// <returns>Paginated list of categories</returns>
    /// <response code="200">Paged categories retrieved successfully</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<CategoryDto>>> GetPagedCategories([FromQuery] CategoryFilterRequest filter)
    {
        var categories = await _categoryService.GetPagedCategoriesAsync(filter);
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves a category by its ID
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <returns>The requested category</returns>
    /// <response code="200">Category retrieved successfully</response>
    /// <response code="404">Category not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    /// <summary>
    /// Retrieves all items belonging to a category
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <returns>List of items in the category</returns>
    /// <response code="200">Items retrieved successfully</response>
    [HttpGet("{id:int}/items")]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Models.Item.ItemDto>>> GetItemsInCategory(int id)
    {
        var items = await _categoryService.GetItemsByCategoryIdAsync(id);
        return Ok(items);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">Category creation data</param>
    /// <returns>The newly created category</returns>
    /// <response code="201">Category created successfully</response>
    /// <response code="400">Invalid category data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="409">Category with the same name already exists</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequest request)
    {
        var category = await _categoryService.CreateCategoryAsync(request);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <param name="request">Updated category data</param>
    /// <returns>No content if the category was updated successfully</returns>
    /// <response code="204">Category updated successfully</response>
    /// <response code="400">Invalid category data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">Category not found</response>
    /// <response code="409">Category with the same name already exists</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
    {
        await _categoryService.UpdateCategoryAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">Category identifier</param>
    /// <returns>No content if the category was deleted successfully</returns>
    /// <response code="204">Category deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">Operation forbidden</response>
    /// <response code="404">Category not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}