using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

/// <summary>
/// Represents a request to update the user's profile information
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// The updated display name of the user
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = string.Empty;
}