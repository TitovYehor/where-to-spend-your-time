namespace WhereToSpendYourTime.Api.Exceptions.Categories
{
    public sealed class CategoryAlreadyExistsException : ArgumentException
    {
        public CategoryAlreadyExistsException(string name) : base($"Category '{name}' already exists") { }
    }
}