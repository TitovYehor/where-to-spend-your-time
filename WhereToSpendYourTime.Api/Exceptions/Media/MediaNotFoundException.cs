namespace WhereToSpendYourTime.Api.Exceptions.Media;

/// <summary>
/// Thrown when a media object with the specified id cannot be found
/// </summary>
public sealed class MediaNotFoundException : Exception
{
    public MediaNotFoundException(int mediaId)
        : base($"Media with id '{mediaId}' was not found") { }
}