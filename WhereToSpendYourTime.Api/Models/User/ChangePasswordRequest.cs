using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

public class ChangePasswordRequest
{
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;
}
