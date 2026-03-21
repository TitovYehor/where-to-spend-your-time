using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents an application role used for authorization.
/// Extends the default ASP.NET Core Identity role
/// </summary>
public class ApplicationRole : IdentityRole
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}