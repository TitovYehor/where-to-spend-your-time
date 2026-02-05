namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class DemoAccountOperationForbiddenException : Exception
    {
        public DemoAccountOperationForbiddenException() : base("Demo account is not allowed to make that operation") { }
    }
}