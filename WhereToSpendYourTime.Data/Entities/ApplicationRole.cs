using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}