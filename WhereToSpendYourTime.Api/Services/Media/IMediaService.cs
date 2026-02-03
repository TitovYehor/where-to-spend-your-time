using WhereToSpendYourTime.Api.Models.Media;

namespace WhereToSpendYourTime.Api.Services.Media;

public interface IMediaService
{
    Task<MediaDto> UploadAsync(CreateMediaDto dto);

    Task DeleteAsync(int mediaId);
}