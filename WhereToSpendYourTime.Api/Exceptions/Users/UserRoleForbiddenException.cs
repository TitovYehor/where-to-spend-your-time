namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserRoleForbiddenException : Exception
    {
        public UserRoleForbiddenException() : base("Forbidden to change role of that user") { }
    }
}