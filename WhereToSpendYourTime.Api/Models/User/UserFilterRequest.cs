namespace WhereToSpendYourTime.Api.Models.User;

public class UserFilterRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;

    public string? Role { get; set; } = string.Empty;

    public string? Search { get; set; } = string.Empty;
}
