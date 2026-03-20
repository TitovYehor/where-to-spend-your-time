using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

/// <summary>
/// Represents a request to change the user's password
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// The current password of the user
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// The new password that will replace the current one
    /// </summary>
    [Required]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}