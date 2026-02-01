namespace WhereToSpendYourTime.Api.Exceptions.Reviews
{
    public sealed class ReviewForbiddenException : Exception
    {
        public ReviewForbiddenException() : base("User is not allowed to modify this review") { }
    }
}