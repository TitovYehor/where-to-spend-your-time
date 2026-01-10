using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Pagination;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Comment;

public class CommentService : ICommentService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CommentDto>> GetCommentsByReviewIdAsync(int reviewId)
    {
        return await _db.Comments
            .AsNoTracking()
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PagedResult<CommentDto>> GetPagedCommentsByReviewIdAsync(int reviewId, CommentFilterRequest filter)
    {
        var query = _db.Comments
            .AsNoTracking()
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<PagedResult<CommentDto>> GetPagedCommentsByUserIdAsync(string userId, CommentFilterRequest filter)
    {
        var query = _db.Comments
           .AsNoTracking()
           .Where(c => c.UserId == userId)
           .OrderByDescending(c => c.CreatedAt)
           .ProjectTo<CommentDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<CommentDto?> AddCommentAsync(int reviewId, string userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var reviewExists = await _db.Reviews.AnyAsync(r => r.Id == reviewId);
        if (!reviewExists)
        {
            return null;
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

    public async Task<bool> UpdateCommentAsync(int commentId, string userId, string newContent)
    {
        var comment = await _db.Comments
            .SingleOrDefaultAsync(c => c.Id == commentId);

        if (comment == null || comment.UserId != userId)
        {
            return false;
        }

        comment.Content = newContent;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isManager)
    {
        var comment = await _db.Comments.FindAsync(commentId);

        if (comment == null || (comment.UserId != userId && !isManager))
        {
            return false;
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
}