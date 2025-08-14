using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public UserServiceTests()
    {
        _userManagerMock = MockUserManager<ApplicationUser>();

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

        _service = new UserService(_db, _mapper, _userManagerMock.Object);
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

    [Fact]
    public async Task UpdateProfileAsync_ReturnsTrue_WhenUserExistsAndUpdateSucceeds()
    {
        var user = new ApplicationUser { Id = "user1", DisplayName = "Old Name" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _service.UpdateProfileAsync(user.Id, "New Name");

        Assert.True(result);
        Assert.Equal("New Name", user.DisplayName);
        _userManagerMock.Verify(m => m.UpdateAsync(It.Is<ApplicationUser>(u => u.DisplayName == "New Name")), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_ReturnsFalse_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing"))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _service.UpdateProfileAsync("missing", "New Name");

        Assert.False(result);
        _userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_ReturnsFalse_WhenUpdateFails()
    {
        var user = new ApplicationUser { Id = "user1", DisplayName = "Old Name" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

        var result = await _service.UpdateProfileAsync(user.Id, "New Name");

        Assert.False(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsTrue_WhenSuccess()
    {
        var user = new ApplicationUser { Id = "user1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Success);

        var (succeeded, errors) = await _service.ChangePasswordAsync(user.Id, "old", "new");

        Assert.True(succeeded);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing"))
            .ReturnsAsync((ApplicationUser?)null);

        var (succeeded, errors) = await _service.ChangePasswordAsync("missing", "old", "new");

        Assert.False(succeeded);
        Assert.Single(errors);
        Assert.Equal("User not found", errors.First().Description);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsFalse_WhenChangePasswordFails()
    {
        var user = new ApplicationUser { Id = "user1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Bad password" }));

        var (succeeded, errors) = await _service.ChangePasswordAsync(user.Id, "old", "new");

        Assert.False(succeeded);
        Assert.Single(errors);
        Assert.Equal("Bad password", errors.First().Description);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<TUser>>().Object,
            Array.Empty<IUserValidator<TUser>>(),
            Array.Empty<IPasswordValidator<TUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<TUser>>>().Object
        );
    }
}
