using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;
public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IReadOnlyList<BankAccount>> GetCustomerBankAccountsAsync(Guid customerId);
    Task<Customer> GetByCustomerNoAsync(string customerNo);
    Task<Customer> GetByIBANAsync(string iban);
    Task<bool> ExistsAsync(string customerNo);
}