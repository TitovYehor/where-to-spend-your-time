using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Services.Category;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<CategoryDto>>> GetPagedCategories([FromQuery] CategoryFilterRequest filter)
    {
        var categories = await _categoryService.GetPagedCategoriesAsync(filter);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    [HttpGet("{id}/items")]
    public async Task<ActionResult<IEnumerable<Models.Item.ItemDto>>> GetItemsInCategory(int id)
    {
        var items = await _categoryService.GetItemsByCategoryIdAsync(id);
        return Ok(items);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequest request)
    {
        var category = await _categoryService.CreateCategoryAsync(request);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
    {
        await _categoryService.UpdateCategoryAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}