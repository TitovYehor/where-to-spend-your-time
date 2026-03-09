namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when attempting to create a user
/// with an email that already exists
/// </summary>
public sealed class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists") { }
}