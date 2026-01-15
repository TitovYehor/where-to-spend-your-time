using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Mapping;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Services.Tags;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class TagServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly TagService _service;

    public TagServiceTests()
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

        _service = new TagService(_db, _mapper);
    }

    [Fact]
    public async Task GetTagsAsync_ReturnsAllTags()
    {
        _db.Tags.AddRange(
            new Tag { Name = "Tag1" },
            new Tag { Name = "Tag2" }
        );
        await _db.SaveChangesAsync();

        var result = await _service.GetTagsAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Name == "Tag1");
        Assert.Contains(result, t => t.Name == "Tag2");
    }

    [Fact]
    public async Task GetPagedTagsAsync_ReturnsPagedResults()
    {
        _db.Tags.AddRange(
            new Tag { Name = "Alpha" },
            new Tag { Name = "Beta" },
            new Tag { Name = "Gamma" }
        );
        await _db.SaveChangesAsync();

        var filter = new TagFilterRequest { Page = 1, PageSize = 2 };

        var result = await _service.GetPagedTagsAsync(filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Alpha", result.Items[0].Name);
    }

    [Fact]
    public async Task GetPagedTagsAsync_AppliesSearchFilter()
    {
        _db.Tags.AddRange(
            new Tag { Name = "Books" },
            new Tag { Name = "Movies" },
            new Tag { Name = "Music" }
        );
        await _db.SaveChangesAsync();

        var filter = new TagFilterRequest { Page = 1, PageSize = 10, Search = "Mo" };

        var result = await _service.GetPagedTagsAsync(filter);

        Assert.Single(result.Items);
        Assert.Equal("Movies", result.Items[0].Name);
    }

    [Fact]
    public async Task GetPagedTagsAsync_ReturnsEmpty_WhenNoMatches()
    {
        _db.Tags.AddRange(
            new Tag { Name = "Books" },
            new Tag { Name = "Games" }
        );
        await _db.SaveChangesAsync();

        var filter = new TagFilterRequest { Page = 1, PageSize = 10, Search = "zzz" };

        var result = await _service.GetPagedTagsAsync(filter);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedTagsAsync_ReturnsCorrectPage()
    {
        _db.Tags.AddRange(
            new Tag { Name = "Alpha" },
            new Tag { Name = "Beta" },
            new Tag { Name = "Gamma" },
            new Tag { Name = "Delta" }
        );
        await _db.SaveChangesAsync();

        var filter = new TagFilterRequest { Page = 2, PageSize = 2 };

        var result = await _service.GetPagedTagsAsync(filter);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Delta", result.Items[0].Name);
        Assert.Equal("Gamma", result.Items[result.Items.Count - 1].Name);
    }

    [Fact]
    public async Task GetTagByIdAsync_ReturnsTag_WhenExists()
    {
        var tag = new Tag { Name = "Test" };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.GetTagByIdAsync(tag.Id);

        Assert.NotNull(result);
        Assert.Equal("Test", result!.Name);
    }

    [Fact]
    public async Task GetTagByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _service.GetTagByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTagAsync_ReturnsNull_WhenNameIsEmpty()
    {
        var request = new TagCreateRequest { Name = "" };
        var result = await _service.CreateTagAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTagAsync_ReturnsNull_WhenDuplicateExists()
    {
        _db.Tags.Add(new Tag { Name = "Duplicate" });
        await _db.SaveChangesAsync();

        var request = new TagCreateRequest { Name = "Duplicate" };
        var result = await _service.CreateTagAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTagAsync_CreatesTag_WhenValid()
    {
        var request = new TagCreateRequest { Name = "NewTag" };

        var result = await _service.CreateTagAsync(request);

        Assert.NotNull(result);
        Assert.Equal("NewTag", result!.Name);
        Assert.True(await _db.Tags.AnyAsync(t => t.Name == "NewTag"));
    }

    [Fact]
    public async Task UpdateTagAsync_ReturnsFalse_WhenNameEmpty()
    {
        var result = await _service.UpdateTagAsync(1, new TagUpdateRequest { Name = "" });
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTagAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.UpdateTagAsync(999, new TagUpdateRequest { Name = "New" });
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTagAsync_Updates_WhenValid()
    {
        var tag = new Tag { Name = "Old" };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.UpdateTagAsync(tag.Id, new TagUpdateRequest { Name = "Updated" });

        Assert.True(result);
        var updated = await _db.Tags.FindAsync(tag.Id);
        Assert.Equal("Updated", updated!.Name);
    }

    [Fact]
    public async Task DeleteTagAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _service.DeleteTagAsync(123);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTagAsync_Deletes_WhenFound()
    {
        var tag = new Tag { Name = "DeleteMe" };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteTagAsync(tag.Id);

        Assert.True(result);
        Assert.False(await _db.Tags.AnyAsync(t => t.Id == tag.Id));
    }
}
