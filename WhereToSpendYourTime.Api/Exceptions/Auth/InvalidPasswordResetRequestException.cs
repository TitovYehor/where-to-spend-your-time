namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when a password reset request contains invalid or incomplete data
/// </summary>
public sealed class InvalidPasswordResetRequestException : Exception
{
    public InvalidPasswordResetRequestException(string message) : base(message) { }
}