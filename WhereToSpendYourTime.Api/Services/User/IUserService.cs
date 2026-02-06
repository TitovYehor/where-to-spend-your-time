using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.User;

namespace WhereToSpendYourTime.Api.Services.User;

public interface IUserService
{
    Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();

    Task<PagedResult<ApplicationUserDto>> GetPagedUsersAsync(UserFilterRequest filter);

    Task<ApplicationUserDto> GetProfileAsync(string userId, bool isSelf);

    Task<IEnumerable<string>> GetRolesAsync();

    Task UpdateProfileAsync(string userId, string displayName);

    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    Task UpdateUserRoleAsync(string userId, string newRole);

    Task DeleteUserAsync(string userId);
}