using System;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;
public interface ICreditCardsRepository : IGenericRepository<CreditCard>
{

}
