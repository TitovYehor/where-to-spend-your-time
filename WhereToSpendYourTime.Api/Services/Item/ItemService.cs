using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Item;

public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ItemService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedItemResult> GetFilteredItemsAsync(ItemFilterRequest filter)
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

        if (filter.TagsIds.Count > 0)
        {
            query = query.Where(i => i.ItemTags.Any(it => filter.TagsIds.Contains(it.Tag.Id)));
        }

        var totalCount = await query.CountAsync();

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

        var mappedItems = items.Select(i =>
        {
            var dto = _mapper.Map<ItemDto>(i);
            dto.CategoryName = i.Category?.Name ?? "Unknown";
            dto.AverageRating = i.Reviews.Count != 0 ? i.Reviews.Average(r => r.Rating) : 0;
            return dto;
        }).ToList();

        return new PagedItemResult
        {
            Items = mappedItems,
            TotalCount = totalCount,
        };
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _db.Items
            .Include(i => i.Category)
            .Include(i => i.Reviews)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return null;
        }

        var dto = _mapper.Map<ItemDto>(item);
        dto.CategoryName = item.Category?.Name ?? "Unknown";
        dto.AverageRating = item.Reviews.Count != 0 ? item.Reviews.Average(r => r.Rating) : 0;
        return dto;
    }

    public async Task<bool> AddTagForItem(int id, string tagName)
    {
        var item = await _db.Items
            .Include(i => i.ItemTags)
            .ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return false;
        }

        tagName = tagName.Trim();
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return false;
        }

        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());
        if (tag == null)
        {
            tag = new Tag { Name = tagName };
            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }

        bool alreadyTagged = item.ItemTags.Any(it => it.TagId == tag.Id);
        if (alreadyTagged)
        {
            return false;
        }

        item.ItemTags.Add(new ItemTag
        {
            ItemId = item.Id,
            TagId = tag.Id
        });


        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveTagFromItem(int id, string tagName)
    {
        var item = await _db.Items
            .Include(i => i.ItemTags)
            .ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return false;
        }

        tagName = tagName.Trim();
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return false;
        }

        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());
        if (tag == null)
        {
            return false;
        }

        var itemTag = item.ItemTags.FirstOrDefault(it => it.TagId == tag.Id);
        if (itemTag == null)
        {
            return false;
        }

        item.ItemTags.Remove(itemTag);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ItemDto?> CreateAsync(ItemCreateRequest request)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return null;
        }

        var item = new Data.Entities.Item
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<ItemDto>(item);
        dto.CategoryName = (await _db.Categories.FindAsync(item.CategoryId))?.Name ?? "Unknown";
        dto.AverageRating = 0;
        return dto;
    }

    public async Task<bool> UpdateAsync(int id, ItemUpdateRequest request)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return false;
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.CategoryId = request.CategoryId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
