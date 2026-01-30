namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Invalid user credentials") { }
    }
}