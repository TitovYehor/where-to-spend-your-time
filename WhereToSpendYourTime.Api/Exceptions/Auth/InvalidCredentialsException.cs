namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when user authentication fails due to invalid credentials
/// </summary>
public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid user credentials") { }
}