namespace WhereToSpendYourTime.Api.Exceptions.Categories;

/// <summary>
/// Thrown when provided category data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidCategoryException : ArgumentException
{
    public InvalidCategoryException(string message) : base(message) { }
}