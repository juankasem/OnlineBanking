using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;
public class DebitCardsRepository : GenericRepository<DebitCard>, IDebitCardsRepository
{
    public DebitCardsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
}
