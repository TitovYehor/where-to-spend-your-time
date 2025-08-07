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
        var categories = await _db.Categories
            .AsNoTracking()
            .Select(cat => _mapper.Map<CategoryDto>(cat))
            .ToListAsync();
        
        return categories;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        var items = await _db.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Reviews)
            .Where(i => i.CategoryId == categoryId)
            .ToListAsync();

        return items.Select(i =>
        {
            var dto = _mapper.Map<ItemDto>(i);
            dto.CategoryName = i.Category?.Name ?? "Unknown";
            dto.AverageRating = i.Reviews.Count != 0 ? i.Reviews.Average(r => r.Rating) : 0;
            return dto;
        });
    }

    public async Task<CategoryDto?> CreateCategoryAsync(CategoryCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var existing = await _db.Categories.AnyAsync(cat => cat.Name.ToLower() == request.Name.ToLower());
        if (existing)
        {
            return null;
        }

        var category = new Data.Entities.Category { Name = request.Name };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return false;
        }

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
