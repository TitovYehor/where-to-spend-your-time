using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Tags;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Tags;

/// <summary>
/// Provides tag management functionality, including querying,
/// filtering, creation, modification, and deletion of tags
/// </summary>
public class TagService : ITagService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TagService(AppDbContext db, IMapper mapper)
    {
        this._db = db ?? throw new ArgumentNullException(nameof(db));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TagDto>> GetTagsAsync()
    {
        return await _db.Tags
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<TagDto> GetTagByIdAsync(int id)
    {
        return await _db.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync()
            ?? throw new TagNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<TagDto> CreateTagAsync(TagCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidTagException("Tag name cannot be empty");
        }

        var name = request.Name.Trim();

        if (await _db.Tags.AnyAsync(t => t.Name == name))
        {
            throw new TagAlreadyExistsException(name);
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

    /// <inheritdoc />
    public async Task UpdateTagAsync(int id, TagUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidTagException("Tag name cannot be empty");
        }

        var tag = await _db.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new TagNotFoundException(id);
        }

        tag.Name = request.Name.Trim();

        await _db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteTagAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id) ?? throw new TagNotFoundException(id);

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
    }
}