using System;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICustomerAccountRepository : IGenericRepository<CustomerBankAccount>
{
    Task<CustomerBankAccount> GetCustomerAccountAsync(Guid customerId, Guid accountId);
}