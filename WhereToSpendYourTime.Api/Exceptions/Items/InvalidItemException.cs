namespace WhereToSpendYourTime.Api.Exceptions.Items
{
    public sealed class InvalidItemException : Exception
    {
        public InvalidItemException(string message) : base(message) { }
    }
}