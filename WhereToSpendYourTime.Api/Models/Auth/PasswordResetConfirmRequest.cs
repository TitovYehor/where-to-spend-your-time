using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Auth;

/// <summary>
/// Represents the request payload for confirming password reset
/// </summary>
public class PasswordResetConfirmRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Token for confirming password reset
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New user password
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}