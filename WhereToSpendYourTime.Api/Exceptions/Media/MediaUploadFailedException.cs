namespace WhereToSpendYourTime.Api.Exceptions.Media
{
    public sealed class MediaUploadFailedException : Exception
    {
        public MediaUploadFailedException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}