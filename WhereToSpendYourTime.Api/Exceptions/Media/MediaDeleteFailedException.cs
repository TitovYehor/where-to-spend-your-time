namespace WhereToSpendYourTime.Api.Exceptions.Media
{
    public sealed class MediaDeleteFailedException : Exception
    {
        public MediaDeleteFailedException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}