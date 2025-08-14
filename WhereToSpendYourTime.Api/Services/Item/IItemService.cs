using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Tags;

namespace WhereToSpendYourTime.Api.Services.Item;

public interface IItemService
{
    Task<PagedItemResult> GetFilteredItemsAsync(ItemFilterRequest filter);

    Task<ItemDto?> GetByIdAsync(int id);

    Task<TagDto?> AddTagForItemAsync(int id, string tagName);

    Task<bool> RemoveTagFromItemAsync(int id, string tagName);

    Task<ItemDto?> CreateAsync(ItemCreateRequest request);
    
    Task<bool> UpdateAsync(int id, ItemUpdateRequest request);
    
    Task<bool> DeleteAsync(int id);
}
