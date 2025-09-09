using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Services.Media;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.ShareLib.Enums;

namespace WhereToSpendYourTime.Tests.Services;

public class MediaServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly MediaService _service;

    public MediaServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Data.Entities.Media, MediaDto>();
        });
        _mapper = config.CreateMapper();

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

        _service = new MediaService(_envMock.Object, _db, _mapper);
    }

    [Fact]
    public async Task UploadAsync_Throws_WhenFileIsEmpty()
    {
        var dto = new CreateMediaDto
        {
            ItemId = 1,
            Type = MediaType.Image,
            File = new FormFile(Stream.Null, 0, 0, "file", "empty.png")
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadAsync(dto));
    }

    [Fact]
    public async Task UploadAsync_SavesFileAndCreatesDbRecord()
    {
        var content = new MemoryStream(new byte[] { 1, 2, 3 });
        var formFile = new FormFile(content, 0, content.Length, "file", "test.png");

        var dto = new CreateMediaDto
        {
            ItemId = 123,
            Type = MediaType.Image,
            File = formFile
        };

        var result = await _service.UploadAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(MediaType.Image, result.Type);
        Assert.Contains($"/uploads/items/123/", result.Url);

        var dbMedia = await _db.Media.FirstOrDefaultAsync(m => m.Id == result.Id);
        Assert.NotNull(dbMedia);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenMediaNotFound()
    {
        var result = await _service.DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesMediaAndFile()
    {
        var media = new Data.Entities.Media
        {
            ItemId = 1,
            Type = MediaType.Image,
            Url = "/uploads/items/1/test.png"
        };
        _db.Media.Add(media);
        await _db.SaveChangesAsync();

        var filePath = Path.Combine(_envMock.Object.WebRootPath, "uploads", "items", "1", "test.png");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(filePath, "test content");

        var result = await _service.DeleteAsync(media.Id);

        Assert.True(result);
        Assert.False(File.Exists(filePath));
        Assert.False(await _db.Media.AnyAsync(m => m.Id == media.Id));
    }
}
