using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Stats;
using WhereToSpendYourTime.Api.Services.Stats;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Provides aggregated statistical data for the application dashboard
/// </summary>
/// <remarks>
/// Base route: api/stats
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>
    /// Retrieves aggregated application statistics
    /// </summary>
    /// <returns>
    /// A <see cref="StatsDto"/> object containing:
    /// - Top rated items
    /// - Most reviewed items
    /// - Top reviewers
    /// - Recent reviews
    /// </returns>
    /// <response code="200">Statistics successfully retrieved</response>
    /// <response code="500">An unexpected server error occurred</response>
    [HttpGet]
    [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StatsDto>> GetStats()
    {
        var stats = await _statsService.GetStatsAsync();
        return Ok(stats);
    }
}