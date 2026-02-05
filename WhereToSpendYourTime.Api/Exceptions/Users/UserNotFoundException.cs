namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserNotFoundException : Exception
    {
        public UserNotFoundException(string userId) : base($"User with id '{userId}' was not found") { }
    }
}