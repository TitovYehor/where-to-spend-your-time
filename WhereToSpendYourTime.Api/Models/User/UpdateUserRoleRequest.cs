using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

public class UpdateUserRoleRequest
{
    [Required]
    public string Role { get; set; } = string.Empty;
}
