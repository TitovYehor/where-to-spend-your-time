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
}
