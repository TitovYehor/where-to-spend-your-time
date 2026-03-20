using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

/// <summary>
/// Represents a request to update a user's role
/// </summary>
public class UpdateUserRoleRequest
{
    /// <summary>
    /// The role that will be assigned to the user
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;
}