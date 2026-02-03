namespace WhereToSpendYourTime.Api.Exceptions.Media
{
    public sealed class InvalidMediaException : Exception
    {
        public InvalidMediaException(string message) : base(message) { }
    }
}