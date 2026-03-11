namespace WhereToSpendYourTime.Api.Exceptions.Reviews;

/// <summary>
/// Thrown when a review with the specified id cannot be found
/// </summary>
public sealed class ReviewNotFoundException : Exception
{
    public ReviewNotFoundException(int reviewId) : base($"Review with id '{reviewId}' was not found") { }
}