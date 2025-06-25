using WhereToSpendYourTime.Api.Models.Stats;

namespace WhereToSpendYourTime.Api.Services.Stats;

public interface IStatsService
{
    Task<StatsDto> GetStatsAsync();
}
