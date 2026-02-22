using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Services.Media;

namespace WhereToSpendYourTime.Api.Controllers;


/// <summary>
/// Provides endpoints for uploading and deleting media files
/// </summary>
/// <remarks>
/// Media files are stored in Azure Blob Storage and metadata is persisted in the database
/// 
/// Base route: api/media
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        this._mediaService = mediaService;
    }

    /// <summary>
    /// Uploads a media file and associates it with an item
    /// </summary>
    /// <param name="dto">
    /// Multipart form-data payload containing:
    /// - File (required)
    /// - ItemId (required)
    /// - Type (media type)
    /// </param>
    /// <returns>The created media metadata</returns>
    /// <response code="200">Media uploaded successfully</response>
    /// <response code="400">Invalid media payload or empty file</response>
    /// <response code="500">Media upload failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(MediaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadMedia([FromForm] CreateMediaDto dto)
    {
        var media = await _mediaService.UploadAsync(dto);
        return Ok(media);
    }

    /// <summary>
    /// Deletes a media file and its associated database record
    /// </summary>
    /// <param name="id">The ID of the media resource</param>
    /// <returns>No content if deletion succeeds</returns>
    /// <response code="204">Media deleted successfully</response>
    /// <response code="404">Media not found</response>
    /// <response code="500">Media deletion failed</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        await _mediaService.DeleteAsync(id);
        return NoContent();
    }
}