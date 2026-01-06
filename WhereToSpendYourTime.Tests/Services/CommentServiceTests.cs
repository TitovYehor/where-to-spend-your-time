using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Services.Comment;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class CommentServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : null));
        });
        _mapper = config.CreateMapper();

        _service = new CommentService(_db, _mapper);
    }

    public delegate Task<PagedResult<CommentDto>> CommentQueryDelegate(CommentService service, string userId, int reviewId, CommentFilterRequest filter);

    public static TheoryData<string, CommentQueryDelegate> GetCommentQueryMethods =>
        new()
        {
            {
                "Review",
                (CommentQueryDelegate)((s, userId, reviewId, filter) =>
                    s.GetPagedCommentsByReviewIdAsync(reviewId, filter))
            },
            {
                "User",
                (CommentQueryDelegate)((s, userId, reviewId, filter) =>
                    s.GetPagedCommentsByUserIdAsync(userId, filter))
            }
        };

    [Fact]
    public async Task GetCommentsByReviewIdAsync_ReturnsOrderedComments()
    {
        var review = new Review { Title = "Rev", Content = "Test", Rating = 5 };
        _db.Reviews.Add(review);

        var user = new ApplicationUser { Id = "u1", DisplayName = "TestUser" };
        _db.Users.Add(user);

        _db.Comments.AddRange(
            new Comment { Content = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-10), Review = review, User = user },
            new Comment { Content = "Second", CreatedAt = DateTime.UtcNow, Review = review, User = user }
        );
        await _db.SaveChangesAsync();

        var result = await _service.GetCommentsByReviewIdAsync(review.Id);

        Assert.Equal(2, result.Count);
        Assert.Equal("Second", result[0].Content);
    }

    [Fact]
    public async Task GetPagedCommentsByReviewIdAsync_Coverage()
    {
        var result = await _service.GetPagedCommentsByReviewIdAsync(
            reviewId: 0,
            new CommentFilterRequest { Page = 1, PageSize = 10 }
        );

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPagedCommentsByUserIdAsync_Coverage()
    {
        var result = await _service.GetPagedCommentsByUserIdAsync(
            userId: "any",
            new CommentFilterRequest { Page = 1, PageSize = 10 }
        );

        Assert.NotNull(result);
    }

    [Theory]
    [MemberData(nameof(GetCommentQueryMethods))]
    public async Task GetPagedCommentsAsync_ReturnsPagedResults(string label, CommentQueryDelegate queryMethod)
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var review = new Review { Title = "Rev", Content = "Test", Rating = 5 };

        var comments = new List<Comment>
        {
            new Comment { Content = "c1", Review = review, User = user, CreatedAt = DateTime.UtcNow },
            new Comment { Content = "c2", Review = review, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(1) },
            new Comment { Content = "c3", Review = review, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(2) }
        };

        _db.Users.Add(user);
        _db.Reviews.Add(review);
        _db.Comments.AddRange(comments);
        await _db.SaveChangesAsync();

        var filter = new CommentFilterRequest { Page = 1, PageSize = 2 };

        var result = await queryMethod(_service, user.Id, review.Id, filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("c3", result.Items[0].Content);
    }

    [Theory]
    [MemberData(nameof(GetCommentQueryMethods))]
    public async Task GetPagedCommentsAsync_ReturnsCorrectPage(string label, CommentQueryDelegate queryMethod)
    {
        var user = new ApplicationUser { Id = "user1", Email = "test@example.com" };
        var review = new Review { Title = "Rev", Content = "Test", Rating = 5 };

        var comments = new List<Comment>
        {
            new Comment { Content = "c1", Review = review, User = user, CreatedAt = DateTime.UtcNow },
            new Comment { Content = "c2", Review = review, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(1) },
            new Comment { Content = "c3", Review = review, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(2) },
            new Comment { Content = "c4", Review = review, User = user, CreatedAt = DateTime.UtcNow.AddMinutes(3) }
        };

        _db.Users.Add(user);
        _db.Reviews.Add(review);
        _db.Comments.AddRange(comments);
        await _db.SaveChangesAsync();

        var filter = new CommentFilterRequest { Page = 2, PageSize = 2 };

        var result = await queryMethod(_service, user.Id, review.Id, filter);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("c2", result.Items[0].Content);
        Assert.Equal("c1", result.Items[result.Items.Count - 1].Content);
    }

    [Fact]
    public async Task AddCommentAsync_AddsComment_WhenReviewExists()
    {
        var review = new Review { Title = "R", Content = "C", Rating = 4 };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.AddCommentAsync(review.Id, "user1", "Nice review");

        Assert.NotNull(result);
        Assert.Equal("Nice review", result!.Content);
        Assert.Single(_db.Comments);
    }

    [Fact]
    public async Task AddCommentAsync_ReturnsNull_WhenReviewNotFound()
    {
        var result = await _service.AddCommentAsync(123, "u", "bad");
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCommentAsync_UpdatesComment_WhenUserIsOwner()
    {
        var comment = new Comment { Content = "Old", UserId = "u123", CreatedAt = DateTime.UtcNow };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var result = await _service.UpdateCommentAsync(comment.Id, "u123", "New");

        Assert.True(result);
        Assert.Equal("New", (await _db.Comments.FirstAsync()).Content);
    }

    [Fact]
    public async Task UpdateCommentAsync_ReturnsFalse_WhenUserIsNotOwner()
    {
        var comment = new Comment { Content = "Protected", UserId = "owner" };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var result = await _service.UpdateCommentAsync(comment.Id, "intruder", "Hacked");

        Assert.False(result);
        Assert.Equal("Protected", (await _db.Comments.FirstAsync()).Content);
    }

    [Fact]
    public async Task DeleteCommentAsync_DeletesComment_WhenOwner()
    {
        var comment = new Comment { Content = "Delete me", UserId = "user" };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteCommentAsync(comment.Id, "user", false);

        Assert.True(result);
        Assert.Empty(_db.Comments);
    }

    [Fact]
    public async Task DeleteCommentAsync_DeletesComment_WhenAdmin()
    {
        var comment = new Comment { Content = "Admin delete", UserId = "user" };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteCommentAsync(comment.Id, "notOwner", true);

        Assert.True(result);
        Assert.Empty(_db.Comments);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsFalse_WhenNotOwnerAndNotAdmin()
    {
        var comment = new Comment { Content = "Blocked", UserId = "trueOwner" };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteCommentAsync(comment.Id, "intruder", false);

        Assert.False(result);
        Assert.Single(_db.Comments);
    }
}
