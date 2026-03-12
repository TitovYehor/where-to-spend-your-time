namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when the current user is not permitted
/// to change role of the specified user
/// </summary>
public sealed class UserRoleForbiddenException : Exception
{
    public UserRoleForbiddenException() : base("Forbidden to change role of that user") { }
}