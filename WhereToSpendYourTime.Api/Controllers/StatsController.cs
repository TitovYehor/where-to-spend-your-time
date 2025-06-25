using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.Stats;
using WhereToSpendYourTime.Api.Services.Stats;
using WhereToSpendYourTime.Data;

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
