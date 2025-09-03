using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;

namespace WhereToSpendYourTime.Api.Services.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    Task<PagedCategoryResult> GetPagedCategoriesAsync(CategoryFilterRequest filter);

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId);
    
    Task<CategoryDto?> CreateCategoryAsync(CategoryCreateRequest request);
    
    Task<bool> UpdateCategoryAsync(int id, CategoryUpdateRequest request);
    
    Task<bool> DeleteCategoryAsync(int id);
}
