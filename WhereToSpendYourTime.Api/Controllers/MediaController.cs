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
    public async Task<IActionResult> UploadMedia([FromForm] CreateMediaDto dto)
    {
        var media = await _mediaService.UploadAsync(dto);
        return Ok(media);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        var deleted = await _mediaService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok(deleted);
    }
}
