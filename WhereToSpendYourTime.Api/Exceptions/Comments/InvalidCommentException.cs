namespace WhereToSpendYourTime.Api.Exceptions.Comments
{
    public sealed class InvalidCommentException : Exception
    {
        public InvalidCommentException(string message) : base(message) { }
    }
}