using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Auth;

/// <summary>
/// Represents the request payload for user authentication
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}