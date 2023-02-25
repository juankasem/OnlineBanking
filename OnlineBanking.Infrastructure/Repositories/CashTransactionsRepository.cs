using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Helpers.Params;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Repositories.Base;

namespace OnlineBanking.Infrastructure.Repositories;

public class CashTransactionsRepository : GenericRepository<CashTransaction>, ICashTransactionsRepository
{
    public CashTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<CashTransaction>> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams ctParams) 
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                  .Where(at => at.Account.AccountNo == accountNoOrIBAN || at.Account.IBAN == accountNoOrIBAN)
                                                  .Include(at => at.Transaction)
                                                  .OrderByDescending(at => at.Transaction.CreatedOn)
                                                  .Where(t => (t.Transaction.TransactionDate - DateTime.Now).TotalDays <= ctParams.TimeScope)
                                                  .Select(at => at.Transaction)
                                                  .AsQueryable();
        
        return await DBHelpers<CashTransaction>.ApplyPagination(query, ctParams.PageNumber, ctParams.PageSize);
                              
    }


    public async Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban, CashTransactionParams ctParams)
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                  .Where(at => at.Account.IBAN == iban)
                                                  .Include(at => at.Transaction)
                                                  .OrderByDescending(at => at.Transaction.CreatedOn)
                                                  .Where(t => (t.Transaction.TransactionDate - DateTime.Now).TotalDays <= ctParams.TimeScope)
                                                  .Select(at => at.Transaction)
                                                  .AsQueryable();
        
        return await DBHelpers<CashTransaction>.ApplyPagination(query, ctParams.PageNumber, ctParams.PageSize);
    }
}
 