using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CashTransactionsRepository : GenericRepository<CashTransaction>, ICashTransactionsRepository
{
    public CashTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IReadOnlyList<CashTransaction>, int)> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams cashTransactionParams)
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                    .Where(at => at.Account.AccountNo == accountNoOrIBAN || at.Account.IBAN == accountNoOrIBAN)
                                                    .Include(at => at.Transaction)
                                                    .ThenInclude(c => c.Currency)
                                                    .OrderBy(at => at.Transaction.CreatedOn)
                                                    .Where(t => t.Transaction.TransactionDate >= DateTime.Now.AddDays(-cashTransactionParams.TimeScope))
                                                    .Select(at => at.Transaction)
                                                    .AsNoTracking()
                                                    .AsQueryable();

        var totalCount = await query.CountAsync();
        var cashTransactions = await ApplyPagination(query, cashTransactionParams.PageNumber, cashTransactionParams.PageSize);

        return (cashTransactions, totalCount);
    }

    public async Task<(IReadOnlyList<CashTransaction>, int)> GetByIBANAsync(string iban, CashTransactionParams cashTransactionParams)
    {
        var query = _dbContext.AccountTransactions.Include(at => at.Account)
                                                    .Where(at => at.Account.IBAN == iban)
                                                    .Include(at => at.Transaction)
                                                    .ThenInclude(c => c.Currency)
                                                    .OrderByDescending(at => at.Transaction.CreatedOn)
                                                    .Where(t => t.Transaction.TransactionDate >= DateTime.Now.AddDays(-cashTransactionParams.TimeScope))
                                                    .Select(at => at.Transaction)
                                                    .AsQueryable();

        var totalCount = await query.CountAsync();
        var cashTransactions = await ApplyPagination(query, cashTransactionParams.PageNumber, cashTransactionParams.PageSize);

        return (cashTransactions, totalCount);
    }
}