namespace WhereToSpendYourTime.Api.Exceptions.Tags
{
    public sealed class TagNotFoundException : KeyNotFoundException
    {
        public TagNotFoundException(int id) : base($"Tag with id '{id}' was not found") { }
    }
}