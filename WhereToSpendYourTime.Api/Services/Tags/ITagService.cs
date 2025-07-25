﻿using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Services.Tags;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetTagsAsync();

    Task<TagDto?> GetTagByIdAsync(int id);

    Task<TagDto?> CreateTagAsync(TagCreateRequest request);

    Task<bool> UpdateTagAsync(int id, TagUpdateRequest request);

    Task<bool> DeleteTagAsync(int id);
}