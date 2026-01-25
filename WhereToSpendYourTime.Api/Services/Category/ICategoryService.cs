using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;

namespace WhereToSpendYourTime.Api.Services.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    Task<PagedResult<CategoryDto>> GetPagedCategoriesAsync(CategoryFilterRequest filter);

    Task<CategoryDto> GetByIdAsync(int id);

    Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId);

    Task<CategoryDto> CreateCategoryAsync(CategoryCreateRequest request);

    Task UpdateCategoryAsync(int id, CategoryUpdateRequest request);

    Task DeleteCategoryAsync(int id);
}