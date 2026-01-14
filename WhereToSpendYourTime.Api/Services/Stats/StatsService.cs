using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            .AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.Title,
                CategoryName = i.Category != null ? i.Category.Name : "Unknown",
                ReviewCount = i.Reviews.Count,
                AverageRating = i.Reviews.Any()
                    ? i.Reviews.Average(r => r.Rating)
                    : 0
            })
            .Where(i => i.ReviewCount >= 3)
            .OrderByDescending(i => i.AverageRating)
            .Take(5)
            .Select(i => new ItemStatDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.CategoryName,
                AverageRating = i.AverageRating,
                ReviewCount = i.ReviewCount
            })
            .ToListAsync();

        var mostReviewedItems = await _db.Items
            .AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.Title,
                CategoryName = i.Category != null ? i.Category.Name : "Unknown",
                ReviewCount = i.Reviews.Count,
                AverageRating = i.Reviews.Any()
                    ? i.Reviews.Average(r => r.Rating)
                    : 0
            })
            .OrderByDescending(i => i.ReviewCount)
            .Take(5)
            .Select(i => new ItemStatDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.CategoryName,
                AverageRating = i.AverageRating,
                ReviewCount = i.ReviewCount
            })
            .ToListAsync();

        var topUsers = await _db.Users
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.DisplayName,
                ReviewCount = u.Reviews.Count,
                Role = u.UserRoles
                    .Select(ur => ur.Role.Name)
                    .OrderBy(r => r)
                    .FirstOrDefault() ?? "User"
            })
            .OrderByDescending(u => u.ReviewCount)
            .Take(5)
            .Select(u => new UserStatDto
            {
                UserId = u.Id,
                DisplayName = u.DisplayName,
                ReviewCount = u.ReviewCount,
                Role = u.Role
            })
            .ToListAsync();

        var recentReviews = await _db.Reviews
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new StatsDto
        {
            TopRatedItems = topRatedItems,
            MostReviewedItems = mostReviewedItems,
            TopReviewers = topUsers,
            RecentReviews = recentReviews
        };
    }
}