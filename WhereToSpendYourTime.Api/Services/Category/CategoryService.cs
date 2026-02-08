using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Exceptions.Categories;
using WhereToSpendYourTime.Api.Extensions;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Pagination;
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
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PagedResult<CategoryDto>> GetPagedCategoriesAsync(CategoryFilterRequest filter)
    {
        var query = _db.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{search}%"));
        }

        query = query.OrderBy(c => c.Name);

        return await query
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(filter.Page, filter.PageSize);
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync()
            ?? throw new CategoryNotFoundException(id);
    }

    public async Task<IEnumerable<ItemDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        return await _db.Items
            .AsNoTracking()
            .Where(i => i.CategoryId == categoryId)
            .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateRequest request)
    {
        ValidateCategory(request.Name);

        var name = request.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name == name))
        {
            throw new CategoryAlreadyExistsException(name);
        }

        var category = new Data.Entities.Category { Name = name };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == category.Id)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .FirstAsync();
    }

    public async Task UpdateCategoryAsync(int id, CategoryUpdateRequest request)
    {
        ValidateCategory(request.Name);

        var category = await _db.Categories.FindAsync(id);
        if (category == null)
        {
            throw new CategoryNotFoundException(id);
        }

        var name = request.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name == name))
        {
            throw new CategoryAlreadyExistsException(name);
        }

        category.Name = name;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id) ?? throw new CategoryNotFoundException(id);

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }

    private static void ValidateCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidCategoryException("Category name cannot be empty");
        }
    }
}