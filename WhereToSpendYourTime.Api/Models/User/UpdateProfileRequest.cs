using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

public class UpdateProfileRequest
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;
}
