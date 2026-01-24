namespace WhereToSpendYourTime.Api.Exceptions.Categories
{
    public sealed class CategoryNotFoundException : KeyNotFoundException
    {
        public CategoryNotFoundException(int id) : base($"Category with id '{id}' was not found") { }
    }
}