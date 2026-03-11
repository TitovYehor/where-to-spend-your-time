namespace WhereToSpendYourTime.Api.Exceptions.Reviews;

/// <summary>
/// Thrown when provided review data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidReviewException : Exception
{
    public InvalidReviewException(string message) : base(message) { }
}