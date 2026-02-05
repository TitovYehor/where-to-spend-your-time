namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserDeleteForbiddenException : Exception
    {
        public UserDeleteForbiddenException() : base("Forbidden to delete that user") { }
    }
}