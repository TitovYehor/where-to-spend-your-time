using System.ComponentModel.DataAnnotations;

namespace WhereToSpendYourTime.Api.Models.User;

/// <summary>
/// Represents filtering and pagination parameters for retrieving users
/// </summary>
public class UserFilterRequest
{
    /// <summary>
    /// The page number to retrieve
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of users to return per page
    /// </summary>
    [Range(1, 30, ErrorMessage = "Page size must be between 1 and 30")]
    public int PageSize { get; set; } = 5;

    /// <summary>
    /// Optional role used to filter users
    /// </summary>
    public string? Role { get; set; } = string.Empty;

    /// <summary>
    /// Optional search text used to filter users by display name or email
    /// </summary>
    public string? Search { get; set; } = string.Empty;
}