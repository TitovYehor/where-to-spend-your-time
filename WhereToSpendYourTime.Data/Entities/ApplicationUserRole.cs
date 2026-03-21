using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Join entity representing the many-to-many relationship
/// between users and roles
/// </summary>
public class ApplicationUserRole : IdentityUserRole<string>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}