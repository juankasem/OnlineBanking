using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CashTransactionsRepository : GenericRepository<CashTransaction>, ICashTransactionsRepository
{
    public CashTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<CashTransaction>> GetByAccountNoAsync(string accountNo)
    {
         return await _dbContext.CashTransactions.Where(c => c.From == accountNo || c.To == accountNo)
                                                    .OrderByDescending(c => c.CreatedOn)
                                                    .ToListAsync();
    }

    public async Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban)
    {
         return await _dbContext.CashTransactions.Where(c => c.From == iban || c.To == iban)
                                                .OrderByDescending(c => c.CreatedOn)
                                                .ToListAsync();
    }
}
