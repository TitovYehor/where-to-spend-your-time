namespace WhereToSpendYourTime.Api.Exceptions.Tags;

/// <summary>
/// Thrown when attempting to create a tag
/// with a name that already exists
/// </summary>
public sealed class TagAlreadyExistsException : ArgumentException
{
    public TagAlreadyExistsException(string name) : base($"Tag '{name}' already exists") { }
}