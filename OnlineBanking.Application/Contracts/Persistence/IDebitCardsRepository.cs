using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IDebitCardsRepository : IGenericRepository<DebitCard>
{

}
