using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}