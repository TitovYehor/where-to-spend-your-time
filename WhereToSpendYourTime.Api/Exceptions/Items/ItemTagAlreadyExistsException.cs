namespace WhereToSpendYourTime.Api.Exceptions.Items
{
    public sealed class ItemTagAlreadyExistsException : Exception
    {
        public ItemTagAlreadyExistsException(int itemId, string tagName) 
            : base($"Tag '{tagName}' is already assigned to item {itemId}") { }
    }
}