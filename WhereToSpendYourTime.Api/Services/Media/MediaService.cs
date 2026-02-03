using AutoMapper;
using Azure.Storage.Blobs;
using WhereToSpendYourTime.Api.Exceptions.Media;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Media;

public class MediaService : IMediaService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly BlobContainerClient _containerClient;

    public MediaService(AppDbContext db, IMapper mapper, BlobContainerClient containerClient)
    {
        this._db = db;
        this._mapper = mapper;
        _containerClient = containerClient ?? throw new ArgumentNullException(nameof(containerClient));
        _containerClient.CreateIfNotExists();
    }

    public async Task<MediaDto> UploadAsync(CreateMediaDto dto)
    {
        if (dto == null)
        {
            throw new InvalidMediaException("Media payload is required");
        }
        if (dto.File == null || dto.File.Length == 0)
        {
            throw new InvalidMediaException("File is empty");
        }

        try
        {
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
        catch (Exception ex)
        {
            throw new MediaUploadFailedException("Failed to upload media", ex);
        }
    }

    public async Task DeleteAsync(int mediaId)
    {
        var media = await _db.Media.FindAsync(mediaId) ?? throw new MediaNotFoundException(mediaId);

        try
        {
            var blobUri = new Uri(media.Url);

            var containerSegment = $"/{_containerClient.Name}/";
            var blobName = blobUri.AbsolutePath.Substring(
                blobUri.AbsolutePath.IndexOf(containerSegment, StringComparison.Ordinal)
                + containerSegment.Length
            );

            var blobClient = _containerClient.GetBlobClient(blobName);

            var deleted = await blobClient.DeleteIfExistsAsync();
            if (!deleted.Value)
            {
                throw new MediaDeleteFailedException("Blob was not found in storage");
            }

            _db.Media.Remove(media);
            await _db.SaveChangesAsync();
        }
        catch (MediaDeleteFailedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MediaDeleteFailedException("Failed to delete media", ex);
        }
    }
}