namespace WhereToSpendYourTime.Api.Exceptions.Comments;

/// <summary>
/// Thrown when a user attempts to modify a comment
/// they are not permitted to access or change
/// </summary>
public sealed class CommentForbiddenException : Exception
{
    public CommentForbiddenException() : base("User is not allowed to modify this comment") { }
}