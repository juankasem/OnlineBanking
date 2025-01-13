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

    public async Task<(IReadOnlyList<CashTransaction>, int)> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams queryParams) 
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                .Where(at => at.Account.AccountNo == accountNoOrIBAN || at.Account.IBAN == accountNoOrIBAN)
                                                .Include(at => at.Transaction)
                                                .OrderByDescending(at => at.Transaction.CreatedOn)
                                                .Where(t => t.Transaction.TransactionDate <= DateTime.Now.AddDays(-queryParams.TimeScope))
                                                .Select(at => at.Transaction)
                                                .AsQueryable();

        var totalCount = await query.CountAsync();

        var cashTransactions = await DBHelpers<CashTransaction>.ApplyPagination(query, queryParams.PageNumber, queryParams.PageSize);

        return (cashTransactions, totalCount);
    }


    public async Task<(IReadOnlyList<CashTransaction>, int)> GetByIBANAsync(string iban, CashTransactionParams queryParams)
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                .Where(at => at.Account.IBAN == iban)
                                                .Include(at => at.Transaction)
                                                .OrderByDescending(at => at.Transaction.CreatedOn)
                                                .Where(t => t.Transaction.TransactionDate <= DateTime.Now.AddDays(-queryParams.TimeScope))
                                                .Select(at => at.Transaction)
                                                .AsQueryable();

        var totalCount = await query.CountAsync();

        var cashTransactions = await DBHelpers<CashTransaction>.ApplyPagination(query, queryParams.PageNumber, queryParams.PageSize);

        return (cashTransactions, totalCount);
    }
}