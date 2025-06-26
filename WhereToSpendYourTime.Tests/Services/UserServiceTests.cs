using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class UserServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ApplicationUser, ApplicationUserDto>();
            cfg.CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : null));
            cfg.CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User!.DisplayName));
        });
        _mapper = config.CreateMapper();

        _service = new UserService(_db, _mapper);
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsNull_WhenUserNotFound()
    {
        var result = await _service.GetProfileAsync("nonexistent", true);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsProfile_WithEmail_WhenIsSelf()
    {
        var review1 = new Review
        {
            Id = 1,
            Title = "Review Title",
            Content = "Good content",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Item = new Item { Title = "Item1" }
        };
        var review2 = new Review
        {
            Id = 2,
            Title = "Another Review",
            Content = "More content",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Item = new Item { Title = "Item2" }
        };
        await _db.Reviews.AddRangeAsync(review1, review2);

        var comment = new Comment
        {
            Id = 1,
            Content = "Nice review!",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Review = review2
        };
        await _db.Comments.AddAsync(comment);

        var user = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            DisplayName = "John Doe",
            Reviews = new List<Review> { review1 },
            Comments = new List<Comment> { comment }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _service.GetProfileAsync(user.Id, true);

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.Email);
        Assert.Single(result.Reviews);
        Assert.Single(result.Comments);
        Assert.Equal("John Doe", result.Reviews[0].Author);
        Assert.Equal("John Doe", result.Comments[0].Author);
    }

    [Fact]
    public async Task GetProfileAsync_HidesEmail_WhenNotSelf()
    {
        var user = new ApplicationUser
        {
            Id = "user2",
            Email = "private@example.com",
            DisplayName = "Jane",
            Reviews = new List<Review>(),
            Comments = new List<Comment>()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _service.GetProfileAsync("user2", isSelf: false);

        Assert.NotNull(result);
        Assert.Null(result.Email);
    }
}
