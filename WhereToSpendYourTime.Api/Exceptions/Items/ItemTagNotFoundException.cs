namespace WhereToSpendYourTime.Api.Exceptions.Items;

/// <summary>
/// Thrown when a tag with the specified id is not assigned to the item
/// </summary>
public sealed class ItemTagNotFoundException : Exception
{
    public ItemTagNotFoundException(int itemId, string tagName)
        : base($"Tag '{tagName}' is not assigned to item {itemId}") { }
}