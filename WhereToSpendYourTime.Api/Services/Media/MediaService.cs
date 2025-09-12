using AutoMapper;
using Azure.Storage.Blobs;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Media;

public class MediaService : IMediaService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly BlobContainerClient _containerClient;

    public MediaService(AppDbContext db, IMapper mapper, IConfiguration config)
    {
        this._db = db;
        this._mapper = mapper;

        var connectionString = config["Azure:BlobConnectionString"];
        var containerName = config["Azure:BlobContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException("Azure Blob Storage is not configured properly");
        }

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<MediaDto> UploadAsync(CreateMediaDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }

        var blobName = $"items/{dto.ItemId}/{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        using (var stream = dto.File.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: false);
        }

        var blobUrl = blobClient.Uri.ToString();

        var media = new Data.Entities.Media
        {
            ItemId = dto.ItemId,
            Type = dto.Type,
            Url = blobUrl
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

        try
        {
            var blobUri = new Uri(media.Url);
            var blobName = blobUri.AbsolutePath.TrimStart('/');

            var containerName = _containerClient.Name;
            if (blobName.StartsWith(containerName + "/"))
            {
                blobName = blobName.Substring(containerName.Length + 1);
            }

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
        catch
        {
            Console.WriteLine("Failed to delete blob from storage");
        }

        _db.Media.Remove(media);
        await _db.SaveChangesAsync();

        return true;
    }
}
