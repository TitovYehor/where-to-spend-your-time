using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Auth;

/// <summary>
/// Represents the request payload for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password.
    /// Must be at least 6 characters long
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Public display name shown to other users
    /// </summary>
    [Required]
    [MinLength(2, ErrorMessage = "Display name must be at least 2 characters long")]
    [MaxLength(40, ErrorMessage = "Display name cannot exceed 40 characters")]
    public string DisplayName { get; set; } = string.Empty;
}