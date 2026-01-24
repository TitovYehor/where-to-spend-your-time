namespace WhereToSpendYourTime.Api.Exceptions.Categories
{
    public sealed class InvalidCategoryException : ArgumentException
    {
        public InvalidCategoryException(string message) : base(message) { }
    }
}