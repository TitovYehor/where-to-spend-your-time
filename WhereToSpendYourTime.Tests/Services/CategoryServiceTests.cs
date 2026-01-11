using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Services.Category;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class CategoryServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "CategoryServiceTestsDb")
            .Options;
        _db = new AppDbContext(options);

        _db.Database.EnsureDeleted();
        _db.Database.EnsureCreated();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryDto>();
            cfg.CreateMap<Item, ItemDto>();
            cfg.CreateMap<Media, MediaDto>();
        });
        _mapper = config.CreateMapper();

        _service = new CategoryService(_db, _mapper);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        _db.Categories.AddRange(new Category { Name = "Books" }, new Category { Name = "Movies" });
        await _db.SaveChangesAsync();

        var result = await _service.GetAllCategoriesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPagedCategoriesAsync_ReturnsPagedResults()
    {
        _db.Categories.AddRange(
            new Category { Name = "Alpha" },
            new Category { Name = "Beta" },
            new Category { Name = "Gamma" }
        );
        await _db.SaveChangesAsync();

        var filter = new CategoryFilterRequest { Page = 1, PageSize = 2 };

        var result = await _service.GetPagedCategoriesAsync(filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Alpha", result.Items[0].Name);
    }

    [Fact]
    public async Task GetPagedCategoriesAsync_AppliesSearchFilter()
    {
        _db.Categories.AddRange(
            new Category { Name = "Books" },
            new Category { Name = "Movies" },
            new Category { Name = "Music" }
        );
        await _db.SaveChangesAsync();

        var filter = new CategoryFilterRequest { Page = 1, PageSize = 10, Search = "Mo" };

        var result = await _service.GetPagedCategoriesAsync(filter);

        Assert.Single(result.Items);
        Assert.Equal("Movies", result.Items[0].Name);
    }

    [Fact]
    public async Task GetPagedCategoriesAsync_ReturnsEmpty_WhenNoMatches()
    {
        _db.Categories.AddRange(
            new Category { Name = "Books" },
            new Category { Name = "Games" }
        );
        await _db.SaveChangesAsync();

        var filter = new CategoryFilterRequest { Page = 1, PageSize = 10, Search = "zzz" };

        var result = await _service.GetPagedCategoriesAsync(filter);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedCategoriesAsync_ReturnsCorrectPage()
    {
        _db.Categories.AddRange(
            new Category { Name = "Alpha" },
            new Category { Name = "Beta" },
            new Category { Name = "Gamma" },
            new Category { Name = "Delta" }
        );
        await _db.SaveChangesAsync();

        var filter = new CategoryFilterRequest { Page = 2, PageSize = 2 };

        var result = await _service.GetPagedCategoriesAsync(filter);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Delta", result.Items[0].Name);
        Assert.Equal("Gamma", result.Items[result.Items.Count - 1].Name);
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsCategory_WhenExists()
    {
        var category = new Category { Name = "Novels" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var categoryDto = await _service.GetByIdAsync(category.Id);

        Assert.NotNull(categoryDto);
        Assert.Equal(category.Name, categoryDto?.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetItemsByCategoryIdAsync_ReturnsItems_WhenCategoryHasItems()
    { 
        var category = new Category { Name = "Cartoons" };
        var item = new Item { Title = "Murphy Law", Category = category };
        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        var result = await _service.GetItemsByCategoryIdAsync(category.Id);

        Assert.Single(result);
        Assert.Equal(item.Title, result.First().Title);
    }

    [Fact]
    public async Task CreateCategoryAsync_AddsCategory()
    {
        var request = new CategoryCreateRequest { Name = "Music" };

        var result = await _service.CreateCategoryAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.True(await _db.Categories.AnyAsync(c => c.Name == request.Name));
    }

    [Fact]
    public async Task UpdateCategoryAsync_Updates_WhenExists()
    {
        var category = new Category { Name = "Old" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var updated = await _service.UpdateCategoryAsync(category.Id, new CategoryUpdateRequest { Name = "New" });

        Assert.True(updated);
        Assert.True(await _db.Categories.AnyAsync(c => c.Name == "New"));
    }

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.UpdateCategoryAsync(123, new CategoryUpdateRequest { Name = "NewName" });
         
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Deletes_WhenExists()
    {
        var category = new Category { Name = "DeleteMe" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteCategoryAsync(category.Id);

        Assert.True(result);
        Assert.False(await _db.Categories.AnyAsync(c => c.Name == "DeleteMe"));
    }

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.DeleteCategoryAsync(999);
        Assert.False(result);
    }
}
