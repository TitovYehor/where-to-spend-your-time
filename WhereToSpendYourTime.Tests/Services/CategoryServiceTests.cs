using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
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

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryDto>();
            cfg.CreateMap<Item, ItemDto>();
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
