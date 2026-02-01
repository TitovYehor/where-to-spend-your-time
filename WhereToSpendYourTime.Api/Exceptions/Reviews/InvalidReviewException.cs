namespace WhereToSpendYourTime.Api.Exceptions.Reviews
{
    public sealed class InvalidReviewException : Exception
    {
        public InvalidReviewException(string message) : base(message) { }
    }
}