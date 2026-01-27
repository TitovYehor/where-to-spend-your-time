namespace WhereToSpendYourTime.Api.Exceptions.Comments
{
    public sealed class CommentNotFoundException : Exception
    {
        public CommentNotFoundException(int commentId) : base($"Comment with id '{commentId}' was not found") { }
    }
}