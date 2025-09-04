using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Api.Models.Review;
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
        var usersDto = await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .Select(u => _mapper.Map<ApplicationUserDto>(u))
            .ToListAsync();

        foreach (var userDto in usersDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            var userRole = await _userManager.GetRolesAsync(user!);
            userDto.Role = userRole?.FirstOrDefault();
        }

        return usersDto.OrderBy(u => u.Role);
    }

    public async Task<PagedResult<ApplicationUserDto>> GetPagedUsersAsync(UserFilterRequest filter)
    {
        var queryDto = _db.Users
            .AsNoTracking()
            .GroupJoin(_db.UserRoles,
                u => u.Id,
                ur => ur.UserId,
                (user, userRoles) => new { user, userRoles })
            .SelectMany(x => x.userRoles.DefaultIfEmpty(),
                (x, userRole) => new { x.user, userRole })
            .GroupJoin(_db.Roles,
                t => t.userRole!.RoleId,
                r => r.Id,
                (t, roles) => new { t.user, roles })
            .SelectMany(t => t.roles.DefaultIfEmpty(),
                (t, role) => new ApplicationUserDto
                {
                    Id = t.user.Id,
                    DisplayName = t.user.DisplayName,
                    Email = t.user.Email,
                    Role = role != null ? role.Name : null
                });

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            queryDto = queryDto.Where(u => u.DisplayName.ToLower().Contains(filter.Search.ToLower()));
        }

        queryDto = queryDto.OrderBy(u => u.Role).ThenBy(u => u.DisplayName).ThenBy(u => u.DisplayName);

        return await queryDto.ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<ApplicationUserDto?> GetProfileAsync(string userId, bool isSelf)
    {
        var user = await _db.Users
            .Include(u => u.Reviews)
                .ThenInclude(r => r.Item)
            .Include(u => u.Comments)
                .ThenInclude(c => c.Review)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        var dto = _mapper.Map<ApplicationUserDto>(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        dto.Role = userRoles?.FirstOrDefault();

        dto.Reviews = user.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                Author = user.DisplayName
            }).ToList();

        dto.Comments = user.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                Author = user.DisplayName,
                CreatedAt = c.CreatedAt,
                ReviewId = c.ReviewId
            }).ToList();

        if (!isSelf)
        {
            dto.Email = null;
            dto.Role = null;
        }

        return dto;
    }

    public async Task<IEnumerable<string?>> GetRolesAsync()
    {
        var roles = await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => r.Name)
            .ToListAsync();
        return roles;
    }

    public async Task<bool> UpdateProfileAsync(string userId, string displayName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
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

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return (result.Succeeded, result.Errors);
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string newRole)
    {
        if (string.IsNullOrEmpty(newRole))
        {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (isAdmin)
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

        var result = await _userManager.AddToRoleAsync(user, newRole);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    { 
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (isAdmin)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}