namespace WhereToSpendYourTime.Api.Exceptions.Items
{
    public sealed class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(int itemId) : base($"Item with id '{itemId}' was not found") { }
    }
}