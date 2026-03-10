namespace WhereToSpendYourTime.Api.Exceptions.Items;

/// <summary>
/// Thrown when attempting to assign a tag to an item
/// that already assigned
/// </summary>
public sealed class ItemTagAlreadyExistsException : Exception
{
    public ItemTagAlreadyExistsException(int itemId, string tagName)
        : base($"Tag '{tagName}' is already assigned to item {itemId}") { }
}