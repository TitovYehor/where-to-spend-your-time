using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Users
{
    public sealed class PasswordChangeFailedException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public PasswordChangeFailedException(IEnumerable<IdentityError> errors)
            : base("Password change failed")
        {
            Errors = errors;
        }
    }
}