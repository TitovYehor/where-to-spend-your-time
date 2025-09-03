using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Tags;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        this._tagService = tagService;        
    }

    [HttpGet]
    public async Task<IActionResult> GetTags()
    { 
        var tags = await _tagService.GetTagsAsync();

        return Ok(tags);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedTags([FromQuery] TagFilterRequest filter)
    {
        var tags = await _tagService.GetPagedTagsAsync(filter);

        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        
        return tag == null ? NotFound() : Ok(tag);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateRequest request)
    {
        var tag = await _tagService.CreateTagAsync(request);
        return tag == null
            ? BadRequest("Invalid tag data or tag already exists")
            : CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, TagUpdateRequest request)
    {
        var result = await _tagService.UpdateTagAsync(id, request);
        return result ? NoContent() : BadRequest("Invalid id or tag data");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var result = await _tagService.DeleteTagAsync(id);
        return result ? NoContent() : NotFound();
    }
}
