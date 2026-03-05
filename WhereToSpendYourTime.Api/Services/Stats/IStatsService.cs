using WhereToSpendYourTime.Api.Models.Stats;

namespace WhereToSpendYourTime.Api.Services.Stats;

/// <summary>
/// Provides aggregated statistical data for the application,
/// including item rankings, user activity, and recent reviews
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Retrieves aggregated statistics for the dashboard,
    /// including top-rated items, most-reviewed items,
    /// top reviewers, and the most recent reviews
    /// </summary>
    /// <returns>
    /// A <see cref="StatsDto"/> containing the compiled statistical data
    /// </returns>
    Task<StatsDto> GetStatsAsync();
}