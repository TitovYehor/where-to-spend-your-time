using WhereToSpendYourTime.Api.Models.Item;

namespace WhereToSpendYourTime.Api.Services.Item;

public interface IItemService
{
    Task<PagedItemResult> GetFilteredItemsAsync(ItemFilterRequest filter);

    Task<ItemDto?> GetByIdAsync(int id);
    
    Task<ItemDto?> CreateAsync(ItemCreateRequest request);
    
    Task<bool> UpdateAsync(int id, ItemUpdateRequest request);
    
    Task<bool> DeleteAsync(int id);
}
