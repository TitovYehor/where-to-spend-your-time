namespace WhereToSpendYourTime.Api.Exceptions.Items;

/// <summary>
/// Thrown when provided item data is invalid
/// or violates validation rules
/// </summary>
public sealed class InvalidItemException : Exception
{
    public InvalidItemException(string message) : base(message) { }
}