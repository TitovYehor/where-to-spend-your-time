using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Comment;
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
        var comments = await _db.Comments
            .Include(c => c.User)
            .Where(c => c.ReviewId == reviewId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task<CommentDto?> AddCommentAsync(int reviewId, string userId, string content)
    {
        var reviewExists = await _db.Reviews.AnyAsync(r => r.Id == reviewId);
        if (!reviewExists) return null;

        var comment = new Data.Entities.Comment
        {
            Content = content,
            CreatedAt = DateTime.UtcNow,
            ReviewId = reviewId,
            UserId = userId
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<bool> UpdateCommentAsync(int commentId, string userId, string newContent)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null || comment.UserId != userId)
        {
            return false;
        }

        comment.Content = newContent;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment == null || (comment.UserId != userId && !isAdmin))
        {
            return false;
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
}
