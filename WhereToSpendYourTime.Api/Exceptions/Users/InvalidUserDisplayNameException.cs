namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class InvalidUserDisplayNameException : Exception
    {
        public InvalidUserDisplayNameException(string message) : base(message) { }
    }
}