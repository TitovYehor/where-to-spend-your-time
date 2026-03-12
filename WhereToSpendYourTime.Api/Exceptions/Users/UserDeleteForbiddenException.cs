namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when the current user is not permitted
/// to delete the specified user
/// </summary>
public sealed class UserDeleteForbiddenException : Exception
{
    public UserDeleteForbiddenException() : base("Forbidden to delete that user") { }
}