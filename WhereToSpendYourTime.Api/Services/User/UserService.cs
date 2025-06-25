using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.User;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UserService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
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
                CreatedAt = c.CreatedAt
            }).ToList();

        if (!isSelf)
        {
            dto.Email = null;
        }

        return dto;
    }
}
