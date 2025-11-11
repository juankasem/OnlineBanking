using Microsoft.Identity.Client;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class BankAccountRepository : GenericRepository<BankAccount>, IBankAccountRepository
{
    public BankAccountRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IReadOnlyList<BankAccount>, int)> GetAllBankAccountsAsync(BankAccountParams bankAccountParams)
    {
        var query = _dbContext.BankAccounts
                             .Include(c => c.Currency)
                             .Include(c => c.Branch)
                             .Include(b => b.CreditCards)
                             .Include(b => b.DebitCards)
                             .Include(b => b.FastTransactions)
                             .AsQueryable();

        var totalCount = await query.CountAsync();
        var bankAccounts = await ApplyPagination(query, bankAccountParams.PageNumber, bankAccountParams.PageSize);

        return (bankAccounts, totalCount);
    }

    public async Task<(IReadOnlyList<BankAccount>, int)> GetBankAccountsByCustomerNoAsync(string customerNo, BankAccountParams bankAccountParams)
    {
        var query = _dbContext.CustomerBankAccounts
                                .Where(cba => cba.Customer.CustomerNo == customerNo)
                                .Include(c => c.Customer)
                                .Include(cba => cba.BankAccount)
                                .ThenInclude(c => c.Currency)
                                .Include(b => b.BankAccount)
                                .ThenInclude(c => c.Branch)
                                .Include(b => b.BankAccount)
                                .ThenInclude(b => b.CreditCards)
                                .Include(b => b.BankAccount)
                                .ThenInclude(b => b.DebitCards)
                                .Include(b => b.BankAccount)
                                .ThenInclude(b => b.FastTransactions)
                                .Select(cba => cba.BankAccount)
                                .AsQueryable();

        var totalCount = await query.CountAsync();
        var bankAccounts = await ApplyPagination(query, bankAccountParams.PageNumber, bankAccountParams.PageSize);

        return (bankAccounts, totalCount);
    }

    public async Task<BankAccount> GetByAccountNoOrIBANAsync(string ibanOrAccountNo) =>
                  await _dbContext.BankAccounts.Where(b => b.AccountNo == ibanOrAccountNo || b.IBAN == ibanOrAccountNo)
                                        .Include(b => b.Branch)
                                        .Include(b => b.Currency)
                                        .Include(b => b.BankAccountOwners)
                                        .ThenInclude(c => c.Customer)
                                        .Include(b => b.FastTransactions)
                                        .Include(b => b.CreditCards)
                                        .Include(b => b.DebitCards)
                                        .FirstOrDefaultAsync();

    public async Task<BankAccount> GetByAccountNoAsync(string accountNo) =>
             await _dbContext.BankAccounts.Where(b => b.AccountNo == accountNo)
                                        .Include(b => b.Branch)
                                        .Include(b => b.Currency)
                                        .Include(b => b.BankAccountOwners)
                                        .ThenInclude(c => c.Customer)
                                        .Include(b => b.FastTransactions)
                                        .Include(b => b.CreditCards)
                                        .Include(b => b.DebitCards)
                                        .FirstOrDefaultAsync();

    public async Task<BankAccount> GetByIBANAsync(string iban) =>
             await _dbContext.BankAccounts.Where(b => b.IBAN == iban)
                                        .Include(b => b.Branch)
                                        .Include(b => b.Currency)
                                        .Include(b => b.BankAccountOwners)
                                        .ThenInclude(c => c.Customer)
                                        .Include(b => b.FastTransactions)
                                        .Include(b => b.CreditCards)
                                        .Include(b => b.DebitCards)
                                        .FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(string iban) =>
             await _dbContext.BankAccounts.AnyAsync(b => b.IBAN == iban);
}