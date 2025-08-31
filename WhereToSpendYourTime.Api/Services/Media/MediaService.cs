using AutoMapper;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Media;

public class MediaService : IMediaService
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MediaService(IWebHostEnvironment env, AppDbContext db, IMapper mapper)
    {
        this._env = env;
        this._db = db;
        this._mapper = mapper;
    }

    public async Task<MediaDto> UploadAsync(CreateMediaDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }

        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "items", dto.ItemId.ToString());
        Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var relativeUrl = $"/uploads/items/{dto.ItemId}/{fileName}";

        var media = new Data.Entities.Media
        {
            ItemId = dto.ItemId,
            Type = dto.Type,
            Url = relativeUrl
        };

        _db.Media.Add(media);
        await _db.SaveChangesAsync();

        return _mapper.Map<MediaDto>(media);
    }

    public async Task<bool> DeleteAsync(int mediaId)
    {
        var media = await _db.Media.FindAsync(mediaId);
        if (media == null)
        {
            return false;
        }

        var filePath = Path.Combine(
            _env.WebRootPath,
            media.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        );

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _db.Media.Remove(media);
        await _db.SaveChangesAsync();

        return true;
    }
}
