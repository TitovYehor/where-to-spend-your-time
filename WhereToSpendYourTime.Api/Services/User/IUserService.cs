using Microsoft.AspNetCore.Identity;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.User;

namespace WhereToSpendYourTime.Api.Services.User;

public interface IUserService
{
    Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();

    Task<PagedResult<ApplicationUserDto>> GetPagedUsersAsync(UserFilterRequest filter);

    Task<ApplicationUserDto?> GetProfileAsync(string userId, bool isSelf);

    Task<IEnumerable<string?>> GetRolesAsync();

    Task<bool> UpdateProfileAsync(string userId, string displayName);

    Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    Task<bool> UpdateUserRoleAsync(string userId, string newRole);

    Task<bool> DeleteUserAsync(string userId);
}
