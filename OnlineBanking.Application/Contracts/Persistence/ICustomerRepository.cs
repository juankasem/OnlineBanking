using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;
public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IReadOnlyList<BankAccount>> GetCustomerBankAccountsAsync(Guid customerId);
    Task<IReadOnlyList<Customer>> GetByIBANAsync(string iban);
    Task<Customer> GetByAppUserIdAsync(string appUserId);
    Task<Customer> GetByCustomerNoAsync(string customerNo);
    Task<bool> ExistsAsync(string customerNo);
}