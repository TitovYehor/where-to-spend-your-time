using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Comments;
using WhereToSpendYourTime.Api.Exceptions.Reviews;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Services.Comment;

/// <summary>
/// Implements comment management logic using Entity Framework Core
/// and AutoMapper projections
/// </summary>
/// <remarks>
/// This service:
/// - Retrieves comments with pagination support
/// - Validates comment content
/// - Ensures related review existence when creating comments
/// - Enforces ownership and role-based authorization for updates and deletions
/// - Throws domain-specific exceptions handled by the global exception handler
/// </remarks>
public class CommentService : ICommentService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<List<CommentDto>> GetCommentsByReviewIdAsync(int reviewId)
    {
        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<PagedResult<CommentDto>> GetPagedCommentsByReviewIdAsync(int reviewId, CommentFilterRequest filter)
    {
        var query = _db.Comments
            .AsNoTracking()
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    /// <inheritdoc />
    public async Task<PagedResult<CommentDto>> GetPagedCommentsByUserIdAsync(string userId, CommentFilterRequest filter)
    {
        var query = _db.Comments
           .AsNoTracking()
           .Where(c => c.UserId == userId)
           .OrderByDescending(c => c.CreatedAt)
           .ProjectTo<CommentDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    /// <inheritdoc />
    public async Task<CommentDto> AddCommentAsync(int reviewId, string userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidCommentException("Comment content cannot be empty");
        }

        var reviewExists = await _db.Reviews.AnyAsync(r => r.Id == reviewId);
        if (!reviewExists)
        {
            throw new ReviewNotFoundException(reviewId);
        }

        var comment = new Data.Entities.Comment
        {
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow,
            ReviewId = reviewId,
            UserId = userId
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }

    /// <inheritdoc />
    public async Task UpdateCommentAsync(int commentId, string userId, string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
        {
            throw new InvalidCommentException("Comment content cannot be empty");
        }

        var comment = await _db.Comments.SingleOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new CommentNotFoundException(commentId);
        }
        if (comment.UserId != userId)
        {
            throw new CommentForbiddenException();
        }

        comment.Content = newContent;
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteCommentAsync(int commentId, ApplicationUser user)
    {
        var comment = await _db.Comments.FindAsync(commentId) ?? throw new CommentNotFoundException(commentId);

        var isAdmin = await _db.UserRoles
            .AnyAsync(r => r.UserId == user.Id && r.Role.Name == "Admin");

        var isModerator = await _db.UserRoles
            .AnyAsync(r => r.UserId == user.Id && r.Role.Name == "Moderator");

        if (comment.UserId != user.Id && !isAdmin && !isModerator)
        {
            throw new CommentForbiddenException();
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
    }
}