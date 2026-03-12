namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when a user with the specified id cannot be found
/// </summary>
public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(string userId) : base($"User with id '{userId}' was not found") { }
}