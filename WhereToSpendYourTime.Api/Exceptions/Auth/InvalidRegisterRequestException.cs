namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class InvalidRegisterRequestException : Exception
    {
        public InvalidRegisterRequestException(string message) : base(message) { }
    }
}