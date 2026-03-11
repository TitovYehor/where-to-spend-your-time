namespace WhereToSpendYourTime.Api.Exceptions.Reviews;

/// <summary>
/// Thrown when a review created by a specific user for a specific item cannot be found
/// </summary>
public sealed class UserItemReviewNotFoundException : Exception
{
    public UserItemReviewNotFoundException(string userId, int itemId)
        : base($"Review for user with id '{userId}' to item with id '{itemId}' was not found") { }
}