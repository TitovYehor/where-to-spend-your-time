using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserRoleUpdateFailedException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public UserRoleUpdateFailedException(IEnumerable<IdentityError> errors)
            : base("User role change failed")
        {
            Errors = errors;
        }
    }
}