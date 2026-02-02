using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Item;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        this._itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<Models.Pagination.PagedResult<ItemDto>>> GetItems([FromQuery] ItemFilterRequest filter)
    {
        var items = await _itemService.GetFilteredItemsAsync(filter);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequest request)
    {
        var item = await _itemService.CreateAsync(request);
        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemUpdateRequest request)
    {
        await _itemService.UpdateAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _itemService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/tags")]
    public async Task<ActionResult<TagDto>> AddTagForItem(int id, [FromBody] TagRequest tag)
    {
        var result = await _itemService.AddTagForItemAsync(id, tag.Name);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/tags/remove/{tagName}")]
    public async Task<IActionResult> RemoveTagFromItem(int id, string tagName)
    {
        await _itemService.RemoveTagFromItemAsync(id, tagName);
        return NoContent();
    }
}