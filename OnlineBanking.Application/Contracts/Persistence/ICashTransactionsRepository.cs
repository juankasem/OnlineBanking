using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICashTransactionsRepository : IGenericRepository<CashTransaction>
{
    Task<IReadOnlyList<CashTransaction>> GetByAccountNoAsync(string accountNo);
    Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban);
}
