namespace WhereToSpendYourTime.Api.Exceptions.Auth;

/// <summary>
/// Thrown when a password reset token is invalid or expired
/// </summary>
public sealed class InvalidPasswordResetTokenException : Exception
{
    public InvalidPasswordResetTokenException() : base("Invalid or expired password reset token") { }
}