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

    public async Task<IReadOnlyList<CashTransaction>> GetByAccountNoAsync(string accountNo) =>
        await _dbContext.AccountTransactions.Include(at => at.Account)
                                            .Where(at => at.Account.AccountNo == accountNo)
                                            .Include(at => at.Transaction)
                                            .OrderByDescending(at => at.Transaction.CreatedOn)
                                            .Select(at => at.Transaction)
                                            .AsNoTracking()
                                            .ToListAsync();


    public async Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban) => 
        await _dbContext.AccountTransactions.Include(at => at.Account)
                                                   .Where(at => at.Account.IBAN == iban)
                                                   .Include(at => at.Transaction)
                                                   .OrderByDescending(at => at.Transaction.CreatedOn)
                                                   .Select(at => at.Transaction)
                                                   .AsNoTracking()
                                                   .ToListAsync();
}
