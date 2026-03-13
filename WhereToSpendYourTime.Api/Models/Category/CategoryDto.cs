namespace WhereToSpendYourTime.Api.Models.Category;

/// <summary>
/// Data transfer object representing a category
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// The unique identifier of the category
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
}