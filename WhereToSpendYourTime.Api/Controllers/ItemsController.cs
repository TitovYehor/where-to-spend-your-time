using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Data;

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
}
