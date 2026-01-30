namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class InvalidLoginRequestException : Exception
    {
        public InvalidLoginRequestException(string message) : base(message) { }
    }
}