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

    [HttpGet]
    public async Task<ActionResult> GetItems([FromQuery] ItemFilterRequest filter)
    { 
        var query = _db.Items
            .Include(i => i.Category)
            .Include(i => i.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(i => i.Title.ToLower().Contains(filter.Search.ToLower()));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == filter.CategoryId.Value);
        }

        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.Descending ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
            "rating" => filter.Descending
                ? query.OrderByDescending(i => i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0)
                : query.OrderBy(i => i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0),
            _ => query.OrderByDescending(i => i.Id)
        };

        int skip = (filter.Page - 1) * filter.PageSize;

        var items = await query
            .Skip(skip)
            .Take(filter.PageSize)
            .ToListAsync();

        var result = items.Select(i =>
        {
            var dto = _mapper.Map<ItemDto>(i);
            dto.CategoryName = i.Category?.Name ?? "Error during name retrieving";
            dto.AverageRating = i.Reviews.Count != 0 ? i.Reviews.Average(r => r.Rating) : 0;
            return dto;
        });

        return Ok(result);
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
            .Include(i => i.Reviews)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        var itemDto = _mapper.Map<ItemDto>(item);

        itemDto.AverageRating = item.Reviews.Count != 0
            ? item.Reviews.Average(r => r.Rating)
            : 0;

        return Ok(itemDto);
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
