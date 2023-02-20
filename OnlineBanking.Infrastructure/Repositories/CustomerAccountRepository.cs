using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CustomerAccountRepository : GenericRepository<CustomerBankAccount>, ICustomerAccountRepository
{
    public CustomerAccountRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CustomerBankAccount> GetCustomerAccountAsync(Guid customerId, Guid accountId) =>
    await _dbContext.CustomerBankAccounts
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.CustomerId == customerId && x.BankAccountId == accountId);
}
