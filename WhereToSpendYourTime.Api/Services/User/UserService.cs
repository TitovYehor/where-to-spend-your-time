using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.User;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(AppDbContext db, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        this._db = db;
        this._mapper = mapper;
        this._userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync()
    {
        var users = await _db.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.DisplayName)
            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return users.OrderBy(u => u.Role);
    }

    public async Task<PagedResult<ApplicationUserDto>> GetPagedUsersAsync(UserFilterRequest filter)
    {
        var query = _db.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Role))
        {
            var role = filter.Role.Trim();
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == role));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(u => u.DisplayName.ToLower().Contains(search));
        }

        query = query.OrderBy(u => u.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault())
                     .ThenBy(u => u.DisplayName);

        var pagedResult = await query
            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);

        return pagedResult;
    }

    public async Task<ApplicationUserDto?> GetProfileAsync(string userId, bool isSelf)
    {
        var userDto = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Reviews)
                .ThenInclude(r => r.Item)
            .Include(u => u.Comments)
                .ThenInclude(c => c.Review)
            .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (userDto != null && !isSelf)
        {
            userDto.Email = null;
        }

        return userDto;
    }

    public async Task<IEnumerable<string?>> GetRolesAsync()
    {
        return await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => r.Name)
            .ToListAsync();
    }

    public async Task<bool> UpdateProfileAsync(string userId, string displayName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.Email == "demo@example.com")
        {
            return false;
        }

        user.DisplayName = displayName;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new[] { new IdentityError { Description = "User not found" } });
        }

        if (user.Email == "demo@example.com")
        {
            return (false, new[] { new IdentityError { Description = "Password change is disabled for the demo account" } });
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return (result.Succeeded, result.Errors);
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole))
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return false;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return false;
            }
        }

        var addResult = await _userManager.AddToRoleAsync(user, newRole);
        return addResult.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}