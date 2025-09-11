using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Item;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class ItemServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Item, ItemDto>();
            cfg.CreateMap<Tag, TagDto>();
            cfg.CreateMap<Media, MediaDto>();
        });
        _mapper = config.CreateMapper();

        _service = new ItemService(_db, _mapper);
    }

    [Fact]
    public async Task GetFilteredItemsAsync_ReturnsFilteredPagedAndSortedItems()
    {
        var cat = new Category { Name = "Books" };
        _db.Categories.Add(cat);

        var items = new List<Item>
        {
            new Item { Title = "First", Description = "D1", Category = cat, Reviews = new List<Review> { new Review { Rating = 4 } } },
            new Item { Title = "Second", Description = "D2", Category = cat, Reviews = new List<Review> { new Review { Rating = 5 } } },
            new Item { Title = "Third", Description = "D3", Category = cat }
        };
        _db.Items.AddRange(items);
        await _db.SaveChangesAsync();

        var filter = new ItemFilterRequest
        {
            Page = 1,
            PageSize = 2,
            SortBy = "rating",
            Descending = true
        };

        var result = await _service.GetFilteredItemsAsync(filter);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Second", result.Items[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsItem_WhenExists()
    {
        var cat = new Category { Name = "Movies" };
        var item = new Item
        {
            Title = "Inception",
            Description = "Mind-bending",
            Category = cat,
            Reviews = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } }
        };
        _db.Categories.Add(cat);
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.GetByIdAsync(item.Id);

        Assert.NotNull(result);
        Assert.Equal("Inception", result!.Title);
        Assert.Equal("Movies", result.CategoryName);
        Assert.Equal(4.5, result.AverageRating);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task AddTagForItemAsync_AddsNewTag_WhenValid()
    {
        var item = new Item { Title = "TestItem" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.AddTagForItemAsync(item.Id, "NewTag");

        Assert.NotNull(result);
        Assert.Equal("NewTag", result!.Name);
        Assert.Single(_db.Tags);
        Assert.Single(_db.ItemTags);
    }

    [Fact]
    public async Task AddTagForItemAsync_ReturnsNull_WhenItemNotFound()
    {
        var result = await _service.AddTagForItemAsync(999, "SomeTag");
        Assert.Null(result);
    }

    [Fact]
    public async Task AddTagForItemAsync_ReturnsNull_WhenTagNameEmpty()
    {
        var item = new Item { Title = "TestItem" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.AddTagForItemAsync(item.Id, "  ");
        Assert.Null(result);
    }

    [Fact]
    public async Task AddTagForItemAsync_ReturnsNull_WhenTagAlreadyExistsForItem()
    {
        var tag = new Tag { Name = "Existing" };
        var item = new Item
        {
            Title = "TestItem",
            ItemTags = new List<ItemTag> { new ItemTag { Tag = tag } }
        };
        _db.Items.Add(item);
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.AddTagForItemAsync(item.Id, "Existing");
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveTagFromItemAsync_RemovesTag_WhenExists()
    {
        var tag = new Tag { Name = "Removable" };
        var item = new Item
        {
            Title = "TestItem",
            ItemTags = new List<ItemTag> { new ItemTag { Tag = tag } }
        };
        _db.Items.Add(item);
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.RemoveTagFromItemAsync(item.Id, "Removable");

        Assert.True(result);
        Assert.Empty(item.ItemTags);
    }

    [Fact]
    public async Task RemoveTagFromItemAsync_ReturnsFalse_WhenItemNotFound()
    {
        var result = await _service.RemoveTagFromItemAsync(999, "SomeTag");
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveTagFromItemAsync_ReturnsFalse_WhenTagNameEmpty()
    {
        var item = new Item { Title = "TestItem" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.RemoveTagFromItemAsync(item.Id, "  ");
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveTagFromItemAsync_ReturnsFalse_WhenTagDoesNotExist()
    {
        var item = new Item { Title = "TestItem" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.RemoveTagFromItemAsync(item.Id, "NoSuchTag");
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveTagFromItemAsync_ReturnsFalse_WhenTagNotAssignedToItem()
    {
        var tag = new Tag { Name = "Unlinked" };
        var item = new Item { Title = "TestItem" };
        _db.Items.Add(item);
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.RemoveTagFromItemAsync(item.Id, "Unlinked");
        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_AddsItem_WhenCategoryExists()
    {
        var category = new Category { Name = "Music" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var request = new ItemCreateRequest
        {
            Title = "Guitar",
            Description = "Acoustic",
            CategoryId = category.Id
        };

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Music", result!.CategoryName);
        Assert.Single(_db.Items);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenCategoryMissing()
    {
        var request = new ItemCreateRequest
        {
            Title = "Drums",
            Description = "Percussion",
            CategoryId = 999
        };

        var result = await _service.CreateAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesItem_WhenExistsAndCategoryValid()
    {
        var cat1 = new Category { Name = "OldCat" };
        var cat2 = new Category { Name = "NewCat" };
        var item = new Item { Title = "Original", Description = "D", Category = cat1 };
        _db.Categories.AddRange(cat1, cat2);
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var request = new ItemUpdateRequest
        {
            Title = "Updated",
            Description = "New desc",
            CategoryId = cat2.Id
        };

        var result = await _service.UpdateAsync(item.Id, request);
        var updatedItem = await _db.Items.FirstAsync();

        Assert.True(result);
        Assert.Equal("Updated", updatedItem.Title);
        Assert.Equal(cat2.Id, updatedItem.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenItemNotFound()
    {
        var request = new ItemUpdateRequest
        {
            Title = "Nothing",
            Description = "Empty",
            CategoryId = 1
        };

        var result = await _service.UpdateAsync(123, request);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenCategoryInvalid()
    {
        var cat = new Category { Name = "Valid" };
        var item = new Item { Title = "Old", Description = "OldDesc", Category = cat };
        _db.Categories.Add(cat);
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.UpdateAsync(item.Id, new ItemUpdateRequest
        {
            Title = "Test",
            Description = "Fail",
            CategoryId = 999
        });

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem_WhenExists()
    {
        var item = new Item { Title = "ToDelete" };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteAsync(item.Id);

        Assert.True(result);
        Assert.Empty(_db.Items);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.DeleteAsync(123);
        Assert.False(result);
    }
}
