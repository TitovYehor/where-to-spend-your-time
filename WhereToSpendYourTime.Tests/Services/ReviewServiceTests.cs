using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Reviews;
using WhereToSpendYourTime.Api.Mapping;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Services.Review;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class ReviewServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ReviewService _service;

    public ReviewServiceTests()
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

        _service = new ReviewService(_db, _mapper);
    }

    public delegate Task<PagedResult<ReviewDto>> ReviewQueryDelegate(ReviewService service, string userId, int itemId, ReviewFilterRequest filter);

    public static TheoryData<string, ReviewQueryDelegate> GetReviewQueryMethods =>
        new()
        {
            {
                "Item",
                (ReviewQueryDelegate)((s, userId, itemId, filter) =>
                    s.GetPagedReviewsForItemAsync(itemId, filter))
            },
            {
                "User",
                (ReviewQueryDelegate)((s, userId, itemId, filter) =>
                    s.GetPagedReviewsForUserAsync(userId, filter))
            }
        };

    [Fact]
    public async Task GetReviewsForItemAsync_ReturnsOrderedReviews()
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var item = new Item { Title = "Item1" };

        var reviews = new List<Review>
        {
            new Review { Title = "Old", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Review { Title = "New", Item = item, User = user, CreatedAt = DateTime.UtcNow }
        };

        _db.Users.Add(user);
        _db.Items.Add(item);
        _db.Reviews.AddRange(reviews);
        await _db.SaveChangesAsync();

        var result = (await _service.GetReviewsForItemAsync(item.Id)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("New", result[0].Title);
    }

    [Fact]
    public async Task GetPagedReviewsForItemAsync_Coverage()
    {
        var result = await _service.GetPagedReviewsForItemAsync(
            itemId: 0,
            new ReviewFilterRequest { Page = 1, PageSize = 2 }
        );

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPagedReviewsForUserAsync_Coverage()
    {
        var result = await _service.GetPagedReviewsForUserAsync(
            userId: "any",
            new ReviewFilterRequest { Page = 1, PageSize = 2 }
        );

        Assert.NotNull(result);
    }

    [Theory]
    [MemberData(nameof(GetReviewQueryMethods))]
    public async Task GetPagedReviewsAsync_ReturnsPagedResults(string label, ReviewQueryDelegate queryMethod)
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var item = new Item { Title = "Item" };

        var reviews = new List<Review>
        {
            new Review { Title = "One", Item = item, User = user, CreatedAt = DateTime.UtcNow },
            new Review { Title = "Two", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(1) },
            new Review { Title = "Three", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(2) }
        };

        _db.Users.Add(user);
        _db.Items.Add(item);
        _db.Reviews.AddRange(reviews);
        await _db.SaveChangesAsync();

        var filter = new ReviewFilterRequest { Page = 1, PageSize = 2 };

        var result = await queryMethod(_service, user.Id, item.Id, filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Three", result.Items[0].Title);
    }

    [Theory]
    [MemberData(nameof(GetReviewQueryMethods))]
    public async Task GetPagedReviewsAsync_ReturnsCorrectPage(string label, ReviewQueryDelegate queryMethod)
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var item = new Item { Title = "Item" };

        var reviews = new List<Review>
        {
            new Review { Title = "One", Item = item, User = user, CreatedAt = DateTime.UtcNow },
            new Review { Title = "Two", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(1) },
            new Review { Title = "Three", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(2) },
            new Review { Title = "Four", Item = item, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(3) }
        };

        _db.Users.Add(user);
        _db.Items.Add(item);
        _db.Reviews.AddRange(reviews);
        await _db.SaveChangesAsync();

        var filter = new ReviewFilterRequest { Page = 2, PageSize = 2 };

        var result = await queryMethod(_service, user.Id, item.Id, filter);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Two", result.Items[0].Title);
        Assert.Equal("One", result.Items[result.Items.Count - 1].Title);
    }

    [Fact]
    public async Task GetMyReviewForItemAsync_ReturnsReview_WhenExists()
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var item = new Item { Title = "Item1" };
        var review = new Review
        {
            Title = "My Review",
            Content = "Content",
            Rating = 5,
            User = user,
            Item = item
        };

        _db.Users.Add(user);
        _db.Items.Add(item);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.GetMyReviewForItemAsync(user.Id, item.Id);

        Assert.Equal("My Review", result.Title);
        Assert.Equal(5, result.Rating);
    }

    [Fact]
    public async Task GetMyReviewForItemAsync_ThrowsUserItemReviewNotFoundException_WhenNotFound()
    {
        var user = new ApplicationUser { Id = "user1" };
        var item = new Item { Title = "Item1" };

        _db.Users.Add(user);
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<UserItemReviewNotFoundException>(() =>
            _service.GetMyReviewForItemAsync(user.Id, item.Id));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsReview_WhenExists()
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var item = new Item { Title = "Item1" };
        var review = new Review
        {
            Title = "Review Title",
            Content = "Some content",
            Rating = 4,
            User = user,
            Item = item,
            Comments = new List<Comment>
            {
                new Comment { Content = "Nice", User = user }
            }
        };

        _db.Users.Add(user);
        _db.Items.Add(item);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.GetByIdAsync(review.Id);

        Assert.Equal("Review Title", result.Title);
        Assert.Equal(4, result.Rating);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsReviewNotFoundException_WhenNotFound()
    {
        await Assert.ThrowsAsync<ReviewNotFoundException>(() =>
            _service.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateReviewAsync_Succeeds_WhenValid()
    {
        var user = new ApplicationUser { Id = "owner" };
        var item = new Item { Title = "Item1" };
        _db.Users.Add(user);
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var request = new ReviewCreateRequest
        {
            ItemId = item.Id,
            Title = "Great",
            Content = "Nice item",
            Rating = 5
        };

        var result = await _service.CreateReviewAsync(user.Id, request);

        Assert.Equal("Great", result.Title);
        Assert.Single(_db.Reviews);
    }

    [Fact]
    public async Task CreateReviewAsync_ThrowsReviewAlreadyExistsException_WhenDuplicate()
    {
        var userId = "user1";
        var item = new Item { Title = "Item1" };
        _db.Items.Add(item);
        _db.Reviews.Add(new Review { Item = item, UserId = userId });
        await _db.SaveChangesAsync();

        var request = new ReviewCreateRequest
        {
            ItemId = item.Id,
            Title = "Duplicate",
            Content = "Nope",
            Rating = 2
        };

        await Assert.ThrowsAsync<ReviewAlreadyExistsException>(() =>
            _service.CreateReviewAsync(userId, request));
    }

    [Fact]
    public async Task UpdateReviewAsync_Succeeds_WhenOwner()
    {
        var review = new Review
        {
            Title = "Initial",
            Content = "Initial",
            Rating = 3,
            UserId = "user1"
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var request = new ReviewUpdateRequest
        {
            Title = "Updated",
            Content = "Updated content",
            Rating = 5
        };

        await _service.UpdateReviewAsync(review.Id, "user1", request);

        var updated = await _db.Reviews.FindAsync(review.Id);
        Assert.Equal("Updated", updated!.Title);
    }

    [Fact]
    public async Task UpdateReviewAsync_ThrowsReviewForbiddenException_WhenNotOwner()
    {
        var review = new Review
        {
            Title = "Initial",
            UserId = "owner"
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var request = new ReviewUpdateRequest
        {
            Title = "Hack",
            Content = "Hack",
            Rating = 1
        };

        await Assert.ThrowsAsync<ReviewForbiddenException>(() =>
            _service.UpdateReviewAsync(review.Id, "notOwner", request));
    }

    [Fact]
    public async Task DeleteReviewAsync_Succeeds_WhenOwner()
    {
        var user = new ApplicationUser { Id = "owner" };
        var review = new Review { UserId = user.Id };

        _db.Users.Add(user);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        await _service.DeleteReviewAsync(review.Id, user);

        Assert.Empty(_db.Reviews);
    }

    [Fact]
    public async Task DeleteReviewAsync_Succeeds_WhenAdmin()
    {
        var user = new ApplicationUser { Id = "admin" };
        var role = new ApplicationRole { Name = "Admin" };

        _db.Users.Add(user);
        _db.Roles.Add(role);
        _db.UserRoles.Add(new ApplicationUserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        var review = new Review { UserId = "someoneElse" };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        await _service.DeleteReviewAsync(review.Id, user);

        Assert.Empty(_db.Reviews);
    }

    [Fact]
    public async Task DeleteReviewAsync_ThrowsReviewForbiddenException_WhenForbidden()
    {
        var user = new ApplicationUser { Id = "notOwner" };
        var review = new Review { UserId = "owner" };

        _db.Users.Add(user);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<ReviewForbiddenException>(() =>
            _service.DeleteReviewAsync(review.Id, user));
    }
}