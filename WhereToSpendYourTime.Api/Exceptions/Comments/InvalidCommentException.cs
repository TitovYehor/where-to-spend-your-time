namespace WhereToSpendYourTime.Api.Exceptions.Comments;

/// <summary>
/// Thrown when provided comment data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidCommentException : Exception
{
    public InvalidCommentException(string message) : base(message) { }
}