using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Mapping;
using WhereToSpendYourTime.Api.Services.Stats;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class StatsServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly StatsService _service;

    public StatsServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new StatsService(_db, _mapper);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsCorrectStats()
    {
        var category = new Category { Name = "Category1" };
        var user1 = new ApplicationUser { Id = "user1", DisplayName = "Alice" };
        var user2 = new ApplicationUser { Id = "user2", DisplayName = "Bob" };
        var item1 = new Item { Title = "Item1", Category = category };
        var item2 = new Item { Title = "Item2", Category = category };

        var reviews = new List<Review>
        {
            new Review { Item = item1, User = user1, Rating = 5, CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new Review { Item = item1, User = user2, Rating = 4, CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new Review { Item = item1, User = user1, Rating = 5, CreatedAt = DateTime.UtcNow.AddHours(-3) },

            new Review { Item = item2, User = user2, Rating = 2, CreatedAt = DateTime.UtcNow.AddHours(-4) }
        };

        _db.Categories.Add(category);
        _db.Users.AddRange(user1, user2);
        _db.Items.AddRange(item1, item2);
        _db.Reviews.AddRange(reviews);
        await _db.SaveChangesAsync();

        var result = await _service.GetStatsAsync();

        Assert.NotNull(result.TopRatedItems);
        Assert.Single(result.TopRatedItems);
        Assert.Equal(item1.Id, result.TopRatedItems[0].Id);
        Assert.Equal(4.67, Math.Round(result.TopRatedItems[0].AverageRating, 2));

        Assert.Equal(2, result.MostReviewedItems.Count);
        Assert.Equal(item1.Id, result.MostReviewedItems[0].Id);
        Assert.Equal(3, result.MostReviewedItems[0].ReviewCount);

        Assert.Equal(2, result.TopReviewers.Count);
        Assert.Contains(result.TopReviewers, u => u.UserId == user1.Id);
        Assert.Contains(result.TopReviewers, u => u.UserId == user2.Id);

        Assert.Equal(4, result.RecentReviews.Count);
        Assert.Equal("Alice", result.RecentReviews[0].Author);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsEmpty_WhenNoData()
    {
        var result = await _service.GetStatsAsync();

        Assert.NotNull(result);
        Assert.Empty(result.TopRatedItems);
        Assert.Empty(result.MostReviewedItems);
        Assert.Empty(result.TopReviewers);
        Assert.Empty(result.RecentReviews);
    }
}