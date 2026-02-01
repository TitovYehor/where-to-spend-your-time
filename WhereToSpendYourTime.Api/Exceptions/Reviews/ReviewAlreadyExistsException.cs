namespace WhereToSpendYourTime.Api.Exceptions.Reviews
{
    public sealed class ReviewAlreadyExistsException : ArgumentException
    {
        public ReviewAlreadyExistsException(int id) : base($"User review for item with id '{id}' already exists") { }
    }
}