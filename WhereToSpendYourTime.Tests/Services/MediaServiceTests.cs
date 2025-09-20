using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    private readonly Mock<BlobContainerClient> _containerMock;
    private readonly Mock<BlobClient> _blobClientMock;
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

        _containerMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();

        _containerMock
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        _service = new MediaService(_db, _mapper, _containerMock.Object);
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

        _blobClientMock.Setup(b => b.Uri).Returns(new Uri("https://fake.blob/items/123/test.png"));
        _blobClientMock.Setup(b => b.UploadAsync(It.IsAny<Stream>(), false, default))
                       .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        var dto = new CreateMediaDto
        {
            ItemId = 123,
            Type = MediaType.Image,
            File = formFile
        };

        var result = await _service.UploadAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(MediaType.Image, result.Type);
        Assert.Contains($"/items/123/", result.Url);

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
            Url = "https://fake.blob/test-container/items/1/test.png"
        };
        _db.Media.Add(media);
        await _db.SaveChangesAsync();

        _blobClientMock
            .Setup(b => b.DeleteIfExistsAsync(
                DeleteSnapshotsOption.None,
                null,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(Response.FromValue(true, null!));

        var result = await _service.DeleteAsync(media.Id);

        Assert.True(result);
        Assert.False(await _db.Media.AnyAsync(m => m.Id == media.Id));
    }
}
