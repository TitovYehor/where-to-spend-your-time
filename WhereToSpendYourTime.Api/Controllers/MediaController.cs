using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Services.Media;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        this._mediaService = mediaService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MediaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadMedia([FromForm] CreateMediaDto dto)
    {
        var media = await _mediaService.UploadAsync(dto);
        return Ok(media);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        await _mediaService.DeleteAsync(id);
        return NoContent();
    }
}