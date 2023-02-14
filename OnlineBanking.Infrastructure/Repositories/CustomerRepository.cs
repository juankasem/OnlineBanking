using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<BankAccount>> GetCustomerBankAccountsAsync(Guid customerId)
    {
        return await _dbContext.BankAccounts.ToListAsync();
    }

    public async Task<Customer> GetByAppUserIdAsync(string appUserId)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
    }

    public async Task<Customer> GetByCustomerNoAsync(string customerNo)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerNo == customerNo);
    }

    public async Task<Customer> GetByIBANAsync(string iban)
    {
        var customerAccounts = await _dbContext.CustomerBankAccounts.FirstOrDefaultAsync(b => b.BankAccount.IBAN == iban);
        return customerAccounts.Customer;
    } 

    public async Task<bool> ExistsAsync(string customerNo)
    {
        return await _dbContext.Customers.AnyAsync(c => c.CustomerNo == customerNo);
    }
}
