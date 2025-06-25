using WhereToSpendYourTime.Api.Models.User;

namespace WhereToSpendYourTime.Api.Services.User;

public interface IUserService
{
    Task<ApplicationUserDto?> GetProfileAsync(string userId, bool isSelf);
}
