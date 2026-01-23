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
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _tagService.GetTagsAsync();
        return Ok(tags);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Models.Pagination.PagedResult<TagDto>>> GetPagedTags([FromQuery] TagFilterRequest filter)
    {
        var tags = await _tagService.GetPagedTagsAsync(filter);
        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTagById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        return Ok(tag);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateRequest request)
    {
        var tag = await _tagService.CreateTagAsync(request);
        return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] TagUpdateRequest request)
    {
        await _tagService.UpdateTagAsync(id, request);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return NoContent();
    }
}