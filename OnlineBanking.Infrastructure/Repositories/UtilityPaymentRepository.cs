using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;
public class UtilityPaymentRepository : GenericRepository<UtilityPayment>, IUtilityPaymentRepository
{
    public UtilityPaymentRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
}
