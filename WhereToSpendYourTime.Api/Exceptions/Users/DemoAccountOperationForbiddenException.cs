namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when a demo account attempts to perform
/// an operation that is restricted for demo users
/// </summary>
public sealed class DemoAccountOperationForbiddenException : Exception
{
    public DemoAccountOperationForbiddenException() : base("Demo account is not allowed to make that operation") { }
}