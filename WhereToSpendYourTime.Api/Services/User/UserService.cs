using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Users;
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

    public async Task<ApplicationUserDto> GetProfileAsync(string userId, bool isSelf)
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
            .FirstOrDefaultAsync()
            ?? throw new UserNotFoundException(userId);

        if (!isSelf)
        {
            userDto.Email = null;
        }

        return userDto;
    }

    public async Task<IEnumerable<string>> GetRolesAsync()
    {
        return await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();
    }

    public async Task UpdateProfileAsync(string userId, string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length < 2)
        {
            throw new InvalidUserDisplayNameException("Display name must be at least 2 characters");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        if (user.Email == "demo@example.com")
        {
            throw new DemoAccountOperationForbiddenException();
        }

        user.DisplayName = displayName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new UserProfileUpdateFailedException(result.Errors);
        }
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new UserNotFoundException(userId);
        if (user.Email == "demo@example.com")
        {
            throw new DemoAccountOperationForbiddenException();
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            throw new PasswordChangeFailedException(result.Errors);
        }
    }

    public async Task UpdateUserRoleAsync(string userId, string newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole))
        {
            throw new InvalidRoleException("Role cannot be null or empty");
        }
        if (newRole != "User" && newRole != "Moderator" && newRole != "Admin")
        {
            throw new InvalidRoleException("Role must be either 'User' or 'Moderator' or 'Admin'");
        }

        if (!await _db.Roles.AnyAsync(r => r.Name == newRole))
        {
            throw new RoleNotFoundException(newRole);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            throw new UserRoleForbiddenException();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count == 1 && currentRoles[0] == newRole)
        {
            return;
        }
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                throw new UserRoleUpdateFailedException(removeResult.Errors);
            }
        }

        var addResult = await _userManager.AddToRoleAsync(user, newRole);
        if (!addResult.Succeeded)
        {
            throw new UserRoleUpdateFailedException(addResult.Errors);
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            throw new UserDeleteForbiddenException();
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new UserDeleteFailedException(result.Errors);
        }
    }
}