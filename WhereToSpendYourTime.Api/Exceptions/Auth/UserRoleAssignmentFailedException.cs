using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Api.Exceptions.Auth
{
    public sealed class UserRoleAssignmentFailedException : Exception
    {
        public string UserId { get; }
        public string Role { get; }
        public IEnumerable<IdentityError> Errors { get; }

        public UserRoleAssignmentFailedException(
            string userId,
            string role,
            IEnumerable<IdentityError> errors)
            : base($"Failed to assign role '{role}' to user '{userId}'")
        {
            UserId = userId;
            Role = role;
            Errors = errors;
        }
    }
}