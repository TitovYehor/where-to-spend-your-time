namespace WhereToSpendYourTime.Api.Exceptions.Reviews
{
    public sealed class ReviewNotFoundException : Exception
    {
        public ReviewNotFoundException(int reviewId) : base($"Review with id '{reviewId}' was not found") { }
    }
}