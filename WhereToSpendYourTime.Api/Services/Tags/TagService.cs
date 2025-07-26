using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        var tags = await _db.Tags
            .AsNoTracking()
            .Select(tag => _mapper.Map<TagDto>(tag))
            .ToListAsync();

        return tags;
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var tag = await _db.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return tag == null ? null : _mapper.Map<TagDto>(tag);
    }

    public async Task<TagDto?> CreateTagAsync(TagCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var existing = await _db.Tags.AnyAsync(t => t.Name.ToLower() == request.Name.ToLower());
        if (existing)
        {
            return null;
        }

        var tag = new Tag { Name = request.Name };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<TagDto>(tag);
        return dto;
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

        tag.Name = request.Name;

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