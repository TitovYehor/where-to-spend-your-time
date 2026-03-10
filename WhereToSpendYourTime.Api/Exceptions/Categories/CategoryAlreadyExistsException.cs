namespace WhereToSpendYourTime.Api.Exceptions.Categories;

/// <summary>
/// Thrown when attempting to create a category
/// with a name that already exists
/// </summary>
public sealed class CategoryAlreadyExistsException : ArgumentException
{
    public CategoryAlreadyExistsException(string name) : base($"Category '{name}' already exists") { }
}