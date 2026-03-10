namespace WhereToSpendYourTime.Api.Exceptions.Media;

/// <summary>
/// Thrown when deleting media fails due to an internal
/// or external storage-related error
/// </summary>
public sealed class MediaDeleteFailedException : Exception
{
    public MediaDeleteFailedException(string message, Exception? inner = null)
        : base(message, inner) { }
}