using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
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
        Assert.Equal("Protected", ( await _db.Comments.FirstAsync()).Content);
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
