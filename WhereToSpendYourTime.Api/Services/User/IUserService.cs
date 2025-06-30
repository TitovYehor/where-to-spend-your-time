using Microsoft.AspNetCore.Identity;
using WhereToSpendYourTime.Api.Models.User;

namespace WhereToSpendYourTime.Api.Services.User;

public interface IUserService
{
    Task<ApplicationUserDto?> GetProfileAsync(string userId, bool isSelf);

    Task<bool> UpdateProfileAsync(string userId, string displayName);

    Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
