
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<IReadOnlyList<BankAccount>> GetAccountsByCustomerNoAsync(string customerNo);
    Task<BankAccount> GetByAccountNoAsync(string accountNo);
    Task<BankAccount> GetByIBANAsync(string iban);
    Task<bool> ExistsAsync(string iban);
}