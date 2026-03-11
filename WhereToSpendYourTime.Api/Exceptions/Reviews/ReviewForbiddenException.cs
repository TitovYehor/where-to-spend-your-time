namespace WhereToSpendYourTime.Api.Exceptions.Reviews;

/// <summary>
/// Thrown when a user attempts to modify a review
/// they are not permitted to access or change
/// </summary>
public sealed class ReviewForbiddenException : Exception
{
    public ReviewForbiddenException() : base("User is not allowed to modify this review") { }
}