
using Microsoft.EntityFrameworkCore;

namespace OnlineBanking.Application.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken token = default)
    {
        //return new PagedList<T>(items, totalCount, pageNumber, pageSize);

        var count = await source.CountAsync(token);
        if (count > 0)
        {
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        return new(Enumerable.Empty<T>().ToList(), 0, 0, 0);
    }

    public static PagedList<T> ToPagedList<T>(this IReadOnlyList<T> items, int count, int pageNumber, int pageSize, CancellationToken token = default)
    {
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
