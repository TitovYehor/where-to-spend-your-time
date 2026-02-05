namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class RoleNotFoundException : Exception
    {
        public RoleNotFoundException(string roleName) : base($"Role with name '{roleName}' was not found") { }
    }
}