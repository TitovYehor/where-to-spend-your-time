namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email)
            : base($"User with email '{email}' already exists") { }
    }
}