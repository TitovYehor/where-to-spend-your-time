namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when a login request contains invalid or incomplete data
/// </summary>
public sealed class InvalidLoginRequestException : Exception
{
    public InvalidLoginRequestException(string message) : base(message) { }
}