using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(2, ErrorMessage = "Display name must be at least 2 characters long")]
    [MaxLength(40, ErrorMessage = "Display name cannot exceed 40 characters")]
    public string DisplayName { get; set; } = string.Empty;
}
