using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Item;
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
    public async Task<IActionResult> GetItems([FromQuery] ItemFilterRequest filter)
    {
        var items = await _itemService.GetFilteredItemsAsync(filter);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateItem(ItemCreateRequest request)
    {
        var item = await _itemService.CreateAsync(request);
        return item == null
            ? BadRequest("Invalid CategoryId")
            : CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, ItemUpdateRequest request)
    {
        var result = await _itemService.UpdateAsync(id, request);
        return result ? NoContent() : BadRequest("Invalid Id or CategoryId");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var result = await _itemService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/tags")]
    public async Task<IActionResult> AddTagForItem(int id, [FromBody] string tagName) 
    {
        var result = await _itemService.AddTagForItem(id, tagName);
        return result ? NoContent() : BadRequest("Could not add tag to the item");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}/tags/{tagName}")]
    public async Task<IActionResult> RemoveTagFromItem(int id, [FromBody] string tagName)
    {
        var result = await _itemService.RemoveTagFromItem(id, tagName);
        return result ? NoContent() : NotFound("Tag or item not found, or tag is not associated with item");
    } 
}