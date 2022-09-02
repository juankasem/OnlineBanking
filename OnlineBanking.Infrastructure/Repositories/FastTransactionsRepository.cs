using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;
public class FastTransactionsRepository : GenericRepository<FastTransaction>, IFastTransactionsRepository
{
    public FastTransactionsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {

    }
    public Task<IReadOnlyList<FastTransaction>> GetByIBANAsync(string iban)
    {
        throw new NotImplementedException();
    }
}
