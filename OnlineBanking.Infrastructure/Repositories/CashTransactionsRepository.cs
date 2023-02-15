using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IReadOnlyList<CashTransaction>> GetByAccountNoAsync(string accountNo, PaginationParams paginationParams) 
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                  .Where(at => at.Account.AccountNo == accountNo)
                                                  .Include(at => at.Transaction)
                                                  .OrderByDescending(at => at.Transaction.CreatedOn)
                                                  .Select(at => at.Transaction)
                                                  .AsQueryable();
        
        return await DBHelpers<CashTransaction>.ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);
                              
    }


    public async Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban, PaginationParams paginationParams)
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                  .Where(at => at.Account.IBAN == iban)
                                                  .Include(at => at.Transaction)
                                                  .OrderByDescending(at => at.Transaction.CreatedOn)
                                                  .Select(at => at.Transaction)
                                                  .AsQueryable();
        
        return await DBHelpers<CashTransaction>.ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);
    }
}
 