
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<(IReadOnlyList<BankAccount>, int)> GetAllBankAccountsAsync(BankAccountParams bankAccountParams);
    Task<(IReadOnlyList<BankAccount>, int)> GetBankAccountsByCustomerNoAsync(string customerNo, BankAccountParams bankAccountParams);
    Task<BankAccount> GetByAccountNoAsync(string accountNo);
    Task<BankAccount> GetByIBANAsync(string iban);
    Task<bool> ExistsAsync(string iban);
}