using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Tags;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides CRUD operations for tags
/// </summary>
/// <remarks>
/// Tags are used to categorize items within the system
///
/// Write operations require the Admin role
/// 
/// Base route: api/tags
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        this._tagService = tagService;
    }

    /// <summary>
    /// Retrieves all tags ordered alphabetically
    /// </summary>
    /// <returns>List of tags</returns>
    /// <response code="200">Tags successfully retrieved</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _tagService.GetTagsAsync();
        return Ok(tags);
    }

    /// <summary>
    /// Retrieves paginated tags with optional search filtering
    /// </summary>
    /// <param name="filter">
    /// Pagination and filtering parameters:
    /// - Page (default: 1)
    /// - PageSize (default: 5)
    /// - Search (optional tag name search)
    /// </param>
    /// <returns>Paged list of tags</returns>
    /// <response code="200">Paged tags successfully retrieved</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Models.Pagination.PagedResult<TagDto>>> GetPagedTags([FromQuery] TagFilterRequest filter)
    {
        var tags = await _tagService.GetPagedTagsAsync(filter);
        return Ok(tags);
    }

    /// <summary>
    /// Retrieves a tag by its identifier
    /// </summary>
    /// <param name="id">Tag identifier</param>
    /// <returns>The requested tag</returns>
    /// <response code="200">Tag found</response>
    /// <response code="404">Tag not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagDto>> GetTagById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        return Ok(tag);
    }

    /// <summary>
    /// Creates a new tag
    /// </summary>
    /// <param name="request">Tag creation payload</param>
    /// <returns>The newly created tag</returns>
    /// <response code="201">Tag successfully created</response>
    /// <response code="400">Invalid tag data</response>
    /// <response code="409">Tag already exists</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateRequest request)
    {
        var tag = await _tagService.CreateTagAsync(request);
        return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
    }

    /// <summary>
    /// Updates an existing tag
    /// </summary>
    /// <param name="id">Tag identifier</param>
    /// <param name="request">Updated tag data</param>
    /// <returns>No content if update succeeds</returns>
    /// <response code="204">Tag successfully updated</response>
    /// <response code="400">Invalid tag data</response>
    /// <response code="404">Tag not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTag(int id, [FromBody] TagUpdateRequest request)
    {
        await _tagService.UpdateTagAsync(id, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a tag
    /// </summary>
    /// <param name="id">Tag identifier</param>
    /// <returns>No content if deletion succeeds</returns>
    /// <response code="204">Tag successfully deleted</response>
    /// <response code="404">Tag not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return NoContent();
    }
}