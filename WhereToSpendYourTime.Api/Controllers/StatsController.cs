using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Services.Stats;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _statsService.GetStatsAsync();

        return Ok(stats);
    }
}
