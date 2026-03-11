namespace WhereToSpendYourTime.Api.Exceptions.Reviews;

/// <summary>
/// Thrown when attempting to create a review for an item
/// which was already reviewed by the same user
/// </summary>
public sealed class ReviewAlreadyExistsException : ArgumentException
{
    public ReviewAlreadyExistsException(int id) : base($"User review for item with id '{id}' already exists") { }
}