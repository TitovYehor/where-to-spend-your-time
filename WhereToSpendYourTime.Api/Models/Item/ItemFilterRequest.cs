﻿namespace WhereToSpendYourTime.Api.Models.Item;

public class ItemFilterRequest
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public List<int?> TagsIds { get; set; } = [];
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
