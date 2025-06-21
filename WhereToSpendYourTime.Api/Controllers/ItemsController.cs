using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ItemsController(AppDbContext db, IMapper mapper)
    {
        this._db = db;
        this._mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemById(int id)
    {
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

        var item = await _db.Items
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ItemDto>(item));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateItem(ItemCreateRequest request)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId");
        }

        var item = new Item
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, ItemUpdateRequest request)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId");
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
