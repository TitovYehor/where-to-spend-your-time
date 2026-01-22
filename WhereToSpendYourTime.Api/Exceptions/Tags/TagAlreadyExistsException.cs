namespace WhereToSpendYourTime.Api.Exceptions.Tags
{
    public sealed class TagAlreadyExistsException : ArgumentException
    {
        public TagAlreadyExistsException(string name) : base($"Tag '{name}' already exists") { }
    }
}