using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Item;

public class ItemService : IItemService
{
    private const double RatingWeight = 2.0;
    private const double ReviewWeight = 0.5;

    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ItemService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PagedResult<ItemDto>> GetFilteredItemsAsync(ItemFilterRequest filter)
    {
        var query = _db.Items.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(i => i.Title.ToLower().Contains(search));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == filter.CategoryId.Value);
        }

        if (filter.TagsIds?.Any() == true)
        {
            query = query.Where(i =>
                filter.TagsIds.All(tagId =>
                    i.ItemTags.Any(it => it.TagId == tagId)));
        }

        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.Descending
                ? query.OrderByDescending(i => i.Title)
                : query.OrderBy(i => i.Title),

            "rating" => filter.Descending
                ? query.OrderByDescending(i =>
                    i.Reviews.Select(r => (double?)r.Rating).Average() ?? 0)
                    .ThenByDescending(i => i.Reviews.Count)
                : query.OrderBy(i =>
                    i.Reviews.Select(r => (double?)r.Rating).Average() ?? 0)
                    .ThenBy(i => i.Reviews.Count),

            "popularity" => filter.Descending
                ? query.OrderByDescending(i =>
                    ((i.Reviews.Select(r => (double?)r.Rating).Average() ?? 0) * RatingWeight)
                    + i.Reviews.Count * ReviewWeight)
                : query.OrderBy(i =>
                    ((i.Reviews.Select(r => (double?)r.Rating).Average() ?? 0) * RatingWeight)
                    + i.Reviews.Count * ReviewWeight),

            _ => query.OrderByDescending(i => i.Id)
        };

        return await query
            .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        return await _db.Items
            .AsNoTracking()
            .Where(i => i.Id == id)
            .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<TagDto?> AddTagForItemAsync(int id, string tagName)
    {
        tagName = tagName.Trim();
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return null;
        }

        var item = await _db.Items
            .Include(i => i.ItemTags)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return null;
        }

        var tag = await _db.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

        if (tag == null)
        {
            tag = new Tag { Name = tagName };
            _db.Tags.Add(tag);
            await _db.SaveChangesAsync();
        }

        if (item.ItemTags.Any(it => it.TagId == tag.Id))
        {
            return null;
        }

        item.ItemTags.Add(new ItemTag
        {
            ItemId = item.Id,
            TagId = tag.Id
        });

        await _db.SaveChangesAsync();
        return _mapper.Map<TagDto>(tag);
    }

    public async Task<bool> RemoveTagFromItemAsync(int id, string tagName)
    {
        tagName = tagName.Trim();
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return false;
        }

        var item = await _db.Items
            .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return false;
        }

        var itemTag = item.ItemTags
            .FirstOrDefault(it => it.Tag.Name.ToLower() == tagName.ToLower());

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
        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == request.CategoryId);

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

        return _mapper.Map<ItemDto>(item);
    }

    public async Task<bool> UpdateAsync(int id, ItemUpdateRequest request)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == request.CategoryId);

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