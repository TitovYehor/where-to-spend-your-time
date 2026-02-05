using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class UserProfileUpdateFailedException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public UserProfileUpdateFailedException(IEnumerable<IdentityError> errors)
            : base("User profile update failed")
        {
            Errors = errors;
        }
    }
}