using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class FastTransactionsRepository : GenericRepository<FastTransaction>, IFastTransactionsRepository
{
    public FastTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
    public async Task<IReadOnlyList<FastTransaction>> GetByIBANAsync(string iban)
    {
        IQueryable<FastTransaction> query = _dbContext.FastTransactions.AsQueryable();

        return await query.Include(ft => ft.BankAccount)
                        .ThenInclude(b => b.Branch)
                        .Where(ft => ft.BankAccount.IBAN == iban)
                        .AsNoTracking()
                        .ToListAsync();
    }
}
