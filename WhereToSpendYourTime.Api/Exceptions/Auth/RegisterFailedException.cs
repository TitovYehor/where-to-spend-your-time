using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class RegisterFailedException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; }

        public RegisterFailedException(IEnumerable<IdentityError> errors)
            : base("User registration failed")
        {
            Errors = errors;
        }
    }
}