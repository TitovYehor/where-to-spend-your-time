using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Data;

namespace WhereToSpendYourTime.Api.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _db.Categories.ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(i => i.Id == id);

        if (category == null)
        {
            return null;
        }

        var dto = _mapper.Map<CategoryDto>(category);
        return dto;
    }

    public async Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        var items = await _db.Items
            .Include(i => i.Category)
            .Where(i => i.CategoryId == categoryId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ItemDto>>(items);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateRequest request)
    {
        var category = new Data.Entities.Category { Name = request.Name };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateRequest request)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        category.Name = request.Name;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return true;
    }
}
