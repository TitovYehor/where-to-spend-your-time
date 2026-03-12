namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when provided user display name is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidUserDisplayNameException : Exception
{
    public InvalidUserDisplayNameException(string message) : base(message) { }
}