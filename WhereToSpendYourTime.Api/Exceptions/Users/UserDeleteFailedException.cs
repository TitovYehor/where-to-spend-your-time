using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserDeleteFailedException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public UserDeleteFailedException(IEnumerable<IdentityError> errors)
            : base("User delete failed")
        {
            Errors = errors;
        }
    }
}