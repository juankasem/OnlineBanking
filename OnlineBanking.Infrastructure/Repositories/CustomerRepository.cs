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
        return await _dbContext.BankAccounts.Where(ba => ba.BankAccountOwners.Where(c => c.CustomerId == customerId).Any()).ToListAsync();
    }

    public async Task<IReadOnlyList<Customer>> GetByIBANAsync(string iban)
    {
        var customers = await _dbContext.CustomerBankAccounts
                                        .Where(b => b.BankAccount.IBAN == iban)
                                        .Select(c => c.Customer)
                                        .AsNoTracking()
                                        .ToListAsync();

        return customers;
    }

    public async Task<Customer> GetByAppUserIdAsync(string appUserId)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.AppUserId == appUserId);
    }

    public async Task<Customer> GetByCustomerNoAsync(string customerNo)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerNo == customerNo);
    }

    public async Task<bool> ExistsAsync(string customerNo)
    {
        return await _dbContext.Customers.AnyAsync(c => c.CustomerNo == customerNo);
    }
}
