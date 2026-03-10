namespace WhereToSpendYourTime.Api.Exceptions.Items;

/// <summary>
/// Thrown when an item with the specified id cannot be found
/// </summary>
public sealed class ItemNotFoundException : Exception
{
    public ItemNotFoundException(int itemId) : base($"Item with id '{itemId}' was not found") { }
}