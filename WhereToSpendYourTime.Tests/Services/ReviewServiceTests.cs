﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            cfg.CreateMap<Review, ReviewDto>();
        });
        _mapper = config.CreateMapper();

        _service = new ReviewService(_db, _mapper);
    }

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
    public async Task CreateReviewAsync_Success_WhenNoExistingReview()
    {
        var userId = "user1";
        var item = new Item { Title = "Item1" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var request = new ReviewCreateRequest
        {
            ItemId = item.Id,
            Title = "Great",
            Content = "Nice item",
            Rating = 5
        };

        var result = await _service.CreateReviewAsync(userId, request);

        Assert.True(result.Success);
        Assert.NotNull(result.Review);
        Assert.Null(result.Error);
        Assert.Single(_db.Reviews);
    }

    [Fact]
    public async Task CreateReviewAsync_Fails_WhenUserAlreadyReviewed()
    {
        var userId = "user1";
        var item = new Item { Title = "Item1" };
        var review = new Review { Item = item, UserId = userId };
        _db.Items.Add(item);
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var request = new ReviewCreateRequest
        {
            ItemId = item.Id,
            Title = "Duplicate",
            Content = "Nope",
            Rating = 2
        };

        var result = await _service.CreateReviewAsync(userId, request);

        Assert.False(result.Success);
        Assert.Null(result.Review);
        Assert.Equal("User already reviewed this item.", result.Error);
    }

    [Fact]
    public async Task UpdateReviewAsync_Succeeds_WhenUserOwnsReview()
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

        var success = await _service.UpdateReviewAsync(review.Id, "user1", request);

        Assert.True(success);
        var updated = await _db.Reviews.FindAsync(review.Id);
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal(5, updated.Rating);
    }

    [Fact]
    public async Task UpdateReviewAsync_Fails_WhenNotOwner()
    {
        var review = new Review
        {
            Title = "Initial",
            UserId = "owner"
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var request = new ReviewUpdateRequest { Title = "Hack", Content = "Hack", Rating = 1 };
        var result = await _service.UpdateReviewAsync(review.Id, "notOwner", request);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteReviewAsync_Succeeds_WhenUserIsOwner()
    {
        var review = new Review { UserId = "owner" };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteReviewAsync(review.Id, "owner", isAdmin: false);

        Assert.True(result);
        Assert.Empty(_db.Reviews);
    }

    [Fact]
    public async Task DeleteReviewAsync_Succeeds_WhenUserIsAdmin()
    {
        var review = new Review { UserId = "user123" };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteReviewAsync(review.Id, "anotherUser", isAdmin: true);

        Assert.True(result);
        Assert.Empty(_db.Reviews);
    }

    [Fact]
    public async Task DeleteReviewAsync_Fails_WhenNotOwnerAndNotAdmin()
    {
        var review = new Review { UserId = "owner" };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteReviewAsync(review.Id, "intruder", isAdmin: false);

        Assert.False(result);
        Assert.Single(_db.Reviews);
    }
}
