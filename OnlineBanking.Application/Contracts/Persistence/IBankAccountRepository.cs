using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<IReadOnlyList<BankAccount>> GetAccountsByCustomerNoAsync(string customerNo);
    Task<BankAccount> GetByAccountNoAsync(string accountNo);
    Task<BankAccount> GetByIBANAsync(string iban);
    Task<BankAccount> GetByIBANWithCashTransactionsAsync(string iban);

    Task<bool> ExistsAsync(string iban);
}