namespace WhereToSpendYourTime.Api.Exceptions.Comments
{
    public sealed class CommentForbiddenException : Exception
    {
        public CommentForbiddenException() : base("User is not allowed to modify this comment") { }
    }
}