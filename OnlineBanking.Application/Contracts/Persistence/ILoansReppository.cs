using System;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface ILoansRepository : IGenericRepository<Loan>
{

}