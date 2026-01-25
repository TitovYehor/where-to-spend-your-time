using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Categories;
using WhereToSpendYourTime.Api.Mapping;
using WhereToSpendYourTime.Api.Models.Category;
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
            cfg.AddProfile<MappingProfile>();
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

        Assert.Equal(category.Name, categoryDto?.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsCategoryNotFoundException_WhenNotExists()
    {
        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            _service.GetByIdAsync(999)
        );
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

        Assert.Equal(request.Name, result.Name);
        Assert.True(await _db.Categories.AnyAsync(c => c.Name == request.Name));
    }

    [Fact]
    public async Task CreateCategoryAsync_ThrowsInvalidCategoryException_WhenNameIsEmpty()
    {
        var request = new CategoryCreateRequest { Name = "" };

        await Assert.ThrowsAsync<InvalidCategoryException>(() =>
            _service.CreateCategoryAsync(request)
        );
    }

    [Fact]
    public async Task CreateCategoryAsync_ThrowsCategoryAlreadyExistsException_WhenDuplicateExists()
    {
        _db.Categories.Add(new Category { Name = "Duplicate" });
        await _db.SaveChangesAsync();

        var request = new CategoryCreateRequest { Name = "Duplicate" };

        await Assert.ThrowsAsync<CategoryAlreadyExistsException>(() =>
            _service.CreateCategoryAsync(request)
        );
    }

    [Fact]
    public async Task UpdateCategoryAsync_Updates_WhenValid()
    {
        var category = new Category { Name = "Old" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        await _service.UpdateCategoryAsync(category.Id, new CategoryUpdateRequest { Name = "New" });

        var updated = await _db.Categories.FindAsync(category.Id);
        Assert.Equal("New", updated!.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ThrowsInvalidCategoryException_WhenNameEmpty()
    {
        await Assert.ThrowsAsync<InvalidCategoryException>(() =>
            _service.UpdateCategoryAsync(1, new CategoryUpdateRequest { Name = "" })
        );
    }

    [Fact]
    public async Task UpdateCategoryAsync_ThrowsCategoryNotFoundException_WhenNotFound()
    {
        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            _service.UpdateCategoryAsync(999, new CategoryUpdateRequest { Name = "New" })
        );
    }

    [Fact]
    public async Task DeleteCategoryAsync_ThrowsCategoryNotFoundException_WhenNotFound()
    {
        await Assert.ThrowsAsync<CategoryNotFoundException>(() =>
            _service.DeleteCategoryAsync(123)
        );
    }

    [Fact]
    public async Task DeleteCategoryAsync_Deletes_WhenExists()
    {
        var category = new Category { Name = "DeleteMe" };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        await _service.DeleteCategoryAsync(category.Id);

        Assert.False(await _db.Categories.AnyAsync(c => c.Name == "DeleteMe"));
    }
}