namespace WhereToSpendYourTime.Api.Exceptions.Items
{
    public sealed class ItemTagNotFoundException : Exception
    {
        public ItemTagNotFoundException(int itemId, string tagName)
            : base($"Tag '{tagName}' is not assigned to item {itemId}") { }
    }
}