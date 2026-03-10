namespace WhereToSpendYourTime.Api.Exceptions.Tags;

/// <summary>
/// Thrown when provided tag data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidTagException : ArgumentException
{
    public InvalidTagException(string message) : base(message) { }
}