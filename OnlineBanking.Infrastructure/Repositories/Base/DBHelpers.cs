using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Infrastructure.Repositories.Base;
public static class DBHelpers<T> where T : class
{
    public static async Task<IReadOnlyList<T>> ApplyPagination(IQueryable<T> query, int pageNumber, int pageSize)
    {
        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
    }
}
