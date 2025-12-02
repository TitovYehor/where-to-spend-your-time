using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.Stats;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Stats;

public class StatsService : IStatsService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public StatsService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<StatsDto> GetStatsAsync()
    {
        var topRatedItems = await _db.Items
            .Where(i => i.Reviews.Count >= 3)
            .OrderByDescending(i => i.Reviews.Average(r => r.Rating))
            .Take(5)
            .Select(i => new ItemStatDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category!.Name,
                AverageRating = i.Reviews.Average(r => r.Rating),
                ReviewCount = i.Reviews.Count
            }).ToListAsync();

        var mostReviewedItems = await _db.Items
            .OrderByDescending(i => i.Reviews.Count)
            .Take(5)
            .Select(i => new ItemStatDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category!.Name,
                AverageRating = i.Reviews.Any() ? i.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = i.Reviews.Count
            }).ToListAsync();

        var topUsers = await _db.Users
            .OrderByDescending(u => u.Reviews.Count)
            .Take(5)
            .Select(u => new UserStatDto
            {
                UserId = u.Id,
                DisplayName = u.DisplayName,
                ReviewCount = u.Reviews.Count,
                Role = u.UserRoles
                   .Select(r => r.Role.Name)
                   .FirstOrDefault() ?? "User"
            }).ToListAsync();

        var recentReviews = await _db.Reviews
            .Include(r => r.User)
            .Include(r => r.Item)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ToListAsync();

        var stats = new StatsDto
        {
            TopRatedItems = topRatedItems,
            MostReviewedItems = mostReviewedItems,
            TopReviewers = topUsers,
            RecentReviews = recentReviews.Select(r =>
            {
                var dto = _mapper.Map<ReviewDto>(r);
                dto.Author = r.User!.DisplayName;
                return dto;
            }).ToList()
        };

        return stats;
    }
}
