using Microsoft.EntityFrameworkCore;
using WhereToSpendYourTime.Api.Models.Pagination;

namespace WhereToSpendYourTime.Api.Extensions;

/// <summary>
/// Provides extension methods for IQueryable to apply
/// common query operations such as pagination
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Converts the specified query into a paged result
    /// </summary>
    /// <remarks>
    /// Executes two database queries:
    /// one to retrieve the total number of records and another
    /// to fetch the requested page of data.
    /// Page numbering starts from 1.
    /// The query should already contain filtering and ordering
    /// before calling this method
    /// </remarks>
    /// <param name="query">
    /// The source query to paginate
    /// </param>
    /// <param name="page">
    /// Page number starting from 1
    /// </param>
    /// <param name="pageSize">
    /// Number of items per page
    /// </param>
    /// <returns>
    /// A PagedResult containing paginated items and total count
    /// </returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount);
    }
}