namespace WhereToSpendYourTime.Api.Exceptions.Media
{
    public sealed class MediaNotFoundException : Exception
    {
        public MediaNotFoundException(int mediaId)
            : base($"Media with id '{mediaId}' was not found") { }
    }
}