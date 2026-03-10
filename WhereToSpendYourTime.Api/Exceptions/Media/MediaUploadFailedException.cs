namespace WhereToSpendYourTime.Api.Exceptions.Media;

/// <summary>
/// Thrown when updating media fails due to an internal
/// or external storage-related error
/// </summary>
public sealed class MediaUploadFailedException : Exception
{
    public MediaUploadFailedException(string message, Exception? inner = null)
        : base(message, inner) { }
}