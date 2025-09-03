namespace WhereToSpendYourTime.Api.Models.Tags;

public class TagFilterRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;

    public string? Search { get; set; } = string.Empty;
}