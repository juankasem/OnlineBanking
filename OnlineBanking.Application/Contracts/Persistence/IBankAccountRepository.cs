
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<IReadOnlyList<BankAccount>> GetAllBankAccountsAsync(BankAccountParams bankAccountParams);
    Task<IReadOnlyList<BankAccount>> GetBankAccountsByCustomerNoAsync(string customerNo);
    Task<BankAccount> GetByAccountNoAsync(string accountNo);
    Task<BankAccount> GetByIBANAsync(string iban);
    Task<bool> ExistsAsync(string iban);
}