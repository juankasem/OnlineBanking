using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class FastTransactionsRepository : GenericRepository<FastTransaction>, IFastTransactionsRepository
{
    public FastTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IReadOnlyList<FastTransaction>, int)> GetByIBANAsync(string iban, FastTransactionParams fastTransactionParams)
    {
        var query = _dbContext.FastTransactions.Include(ft => ft.BankAccount)
                                                .ThenInclude(b => b.Branch)
                                                .Where(ft => ft.BankAccount.IBAN == iban)
                                                .AsQueryable();

        var totalCount = await query.CountAsync();
        var fastTransactions = await ApplyPagination(query, fastTransactionParams.PageNumber, fastTransactionParams.PageSize);

        return (fastTransactions, totalCount);
    }
}