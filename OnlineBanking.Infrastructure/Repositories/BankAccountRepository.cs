
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class BankAccountRepository : GenericRepository<BankAccount>, IBankAccountRepository
{
    public BankAccountRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<BankAccount>> GetAccountsByCustomerNoAsync(string customerNo)
    {
        IQueryable<CustomerBankAccount> query = _dbContext.CustomerBankAccounts.AsQueryable();

        return await query.Where(cba => cba.Customer.CustomerNo == customerNo)
                                .Include(cba => cba.BankAccount)
                                .ThenInclude(c => c.Currency)
                                .Include(b => b.BankAccount)
                                .ThenInclude(c => c.Branch)
                                .Select(cba => cba.BankAccount)
                                .AsNoTracking()
                                .ToListAsync();
    }


    public async Task<BankAccount> GetByAccountNoAsync(string accountNo) =>
    await _dbContext.BankAccounts.Where(b => b.AccountNo == accountNo)
                                    .Include(b => b.Branch)
                                    .Include(b => b.Currency)
                                    .Include(b => b.BankAccountOwners)
                                    .Include(b => b.AccountTransactions)
                                    .Include(b => b.CreditCards)
                                    .Include(b => b.DebitCards)
                                    .FirstOrDefaultAsync();

    public async Task<BankAccount> GetByIBANAsync(string iban) =>
        await _dbContext.BankAccounts.Where(b => b.IBAN == iban)
                                    .Include(b => b.Branch)
                                    .Include(b => b.Currency)
                                    .Include(b => b.BankAccountOwners).Include(b => b.CreditCards)
                                    .Include(b => b.DebitCards)
                                    .FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(string iban) =>
        await _dbContext.BankAccounts.AnyAsync(b => b.IBAN == iban);
}