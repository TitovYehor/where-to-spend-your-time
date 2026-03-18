namespace WhereToSpendYourTime.Api.Models.Tags;

/// <summary>
/// Data transfer object representing a tag
/// </summary>
public class TagDto
{
    /// <summary>
    /// The unique identifier of the tag
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the tag
    /// </summary>
    public string Name { get; set; } = string.Empty;
}