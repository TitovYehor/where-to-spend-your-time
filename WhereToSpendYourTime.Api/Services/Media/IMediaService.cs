using WhereToSpendYourTime.Api.Models.Media;

namespace WhereToSpendYourTime.Api.Services.Media;

/// <summary>
/// Provides operations for uploading and deleting media files
/// associated with items
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Uploads a media file to the storage provider and persists its metadata in the database
    /// </summary>
    /// <param name="dto">
    /// The media creation payload containing the file, associated item id,
    /// and media type information
    /// </param>
    /// <returns>
    /// A <see cref="MediaDto"/> representing the uploaded media resource
    /// </returns>
    /// <exception cref="InvalidMediaException">
    /// Thrown when the provided payload is null or the file is empty
    /// </exception>
    /// <exception cref="MediaUploadFailedException">
    /// Thrown when the upload process fails
    /// </exception>
    Task<MediaDto> UploadAsync(CreateMediaDto dto);

    /// <summary>
    /// Deletes a media resource from both the storage provider and the database
    /// </summary>
    /// <param name="mediaId">
    /// The id of the media resource to delete
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous delete operation
    /// </returns>
    /// <exception cref="MediaNotFoundException">
    /// Thrown when the media resource with the specified id does not exist
    /// </exception>
    /// <exception cref="MediaDeleteFailedException">
    /// Thrown when the deletion process fails either in storage or in the database
    /// </exception>
    Task DeleteAsync(int mediaId);
}