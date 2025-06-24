namespace WhereToSpendYourTime.Api.Models.Stats;

public class ItemStatDto
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    
    public double AverageRating { get; set; }

    public int ReviewCount { get; set; }
}
