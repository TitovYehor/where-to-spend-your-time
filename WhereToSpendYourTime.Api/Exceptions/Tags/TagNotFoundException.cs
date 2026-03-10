namespace WhereToSpendYourTime.Api.Exceptions.Tags;

/// <summary>
/// Thrown when a tag with the specified id cannot be found
/// </summary>
public sealed class TagNotFoundException : KeyNotFoundException
{
    public TagNotFoundException(int id) : base($"Tag with id '{id}' was not found") { }
}