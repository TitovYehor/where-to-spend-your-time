using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Auth;

/// <summary>
/// Represents the request payload for initiating password reset functionality
/// </summary>
public class PasswordResetRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}