﻿namespace WhereToSpendYourTime.Api.Models.Review;

public class ReviewDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
