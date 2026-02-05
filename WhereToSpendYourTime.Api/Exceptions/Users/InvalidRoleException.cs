namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class InvalidRoleException : Exception
    {
        public InvalidRoleException(string message) : base(message) { }
    }
}