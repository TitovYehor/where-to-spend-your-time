namespace WhereToSpendYourTime.Api.Models.Category;

public class PagedCategoryResult
{
    public List<CategoryDto> Categories { get; set; } = [];

    public int TotalCount { get; set; }
}
