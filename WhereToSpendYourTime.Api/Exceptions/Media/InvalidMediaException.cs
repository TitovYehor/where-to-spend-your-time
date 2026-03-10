namespace WhereToSpendYourTime.Api.Exceptions.Media;

/// <summary>
/// Thrown when provided media data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidMediaException : Exception
{
    public InvalidMediaException(string message) : base(message) { }
}