namespace WhereToSpendYourTime.Api.Exceptions.Tags
{
    public sealed class InvalidTagException : ArgumentException
    {
        public InvalidTagException(string message) : base(message) { }
    }
}