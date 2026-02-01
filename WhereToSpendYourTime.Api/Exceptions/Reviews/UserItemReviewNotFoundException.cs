namespace WhereToSpendYourTime.Api.Exceptions.Reviews
{
    public sealed class UserItemReviewNotFoundException : Exception
    {
        public UserItemReviewNotFoundException(string userId, int itemId) 
            : base($"Review for user with id '{userId}' to item with id '{itemId}' was not found") { }
    }
}