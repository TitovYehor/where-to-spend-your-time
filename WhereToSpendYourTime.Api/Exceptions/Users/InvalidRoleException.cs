namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when provided role data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidRoleException : Exception
{
    public InvalidRoleException(string message) : base(message) { }
}