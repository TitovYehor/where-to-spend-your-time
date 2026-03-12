namespace WhereToSpendYourTime.Api.Exceptions.Users;

/// <summary>
/// Thrown when a role with the specified name cannot be found
/// </summary>
public sealed class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string roleName) : base($"Role with name '{roleName}' was not found") { }
}