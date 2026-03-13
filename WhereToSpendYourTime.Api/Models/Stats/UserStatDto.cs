namespace WhereToSpendYourTime.Api.Models.Stats;

/// <summary>
/// Data transfer object representing statistical information about a user
/// </summary>
public class UserStatDto
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the user
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The number of reviews written by the user
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// The role assigned to the user
    /// </summary>
    public string? Role { get; set; }
}