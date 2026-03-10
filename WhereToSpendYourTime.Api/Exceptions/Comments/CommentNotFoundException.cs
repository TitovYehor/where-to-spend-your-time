namespace WhereToSpendYourTime.Api.Exceptions.Comments;

/// <summary>
/// Thrown when a comment with the specified id cannot be found
/// </summary>
public sealed class CommentNotFoundException : Exception
{
    public CommentNotFoundException(int commentId) : base($"Comment with id '{commentId}' was not found") { }
}