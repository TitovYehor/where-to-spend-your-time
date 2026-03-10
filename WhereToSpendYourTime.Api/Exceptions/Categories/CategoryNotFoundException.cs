namespace WhereToSpendYourTime.Api.Exceptions.Categories;

/// <summary>
/// Thrown when a category with the specified id cannot be found.
/// </summary>
public sealed class CategoryNotFoundException : KeyNotFoundException
{
    public CategoryNotFoundException(int id) : base($"Category with id '{id}' was not found") { }
}