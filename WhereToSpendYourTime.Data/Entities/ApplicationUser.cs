using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

/// <summary>
/// Represents a user of the application.
/// Extends the ASP.NET Core Identity user
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Public display name used across the application
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Roles assigned to the user
    /// </summary>
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

    /// <summary>
    /// Reviews created by the user
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    /// <summary>
    /// Comments created by the user
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}