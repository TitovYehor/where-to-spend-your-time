using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Tags;

public class TagService : ITagService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TagService(AppDbContext db, IMapper mapper)
    {
        this._db = db ?? throw new ArgumentNullException(nameof(db));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<TagDto>> GetTagsAsync()
    {
        return await _db.Tags
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PagedResult<TagDto>> GetPagedTagsAsync(TagFilterRequest filter)
    {
        var query = _db.Tags.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(t => EF.Functions.Like(t.Name, $"%{search}%"));
        }

        query = query.OrderBy(t => t.Name);

        return await query
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        return await _db.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<TagDto?> CreateTagAsync(TagCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var name = request.Name.Trim();

        var exists = await _db.Tags.AnyAsync(t => t.Name == name);
        if (exists)
        {
            return null;
        }

        var tag = new Tag { Name = name };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        return await _db.Tags
            .AsNoTracking()
            .Where(t => t.Id == tag.Id)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }

    public async Task<bool> UpdateTagAsync(int id, TagUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return false;
        }

        var tag = await _db.Tags.FindAsync(id);
        if (tag == null)
        {
            return false;
        }

        tag.Name = request.Name.Trim();

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id);

        if (tag == null)
        {
            return false;
        }

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
        return true;
    }
}