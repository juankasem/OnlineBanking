using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICashTransactionsRepository : IGenericRepository<CashTransaction>
{
    Task<IReadOnlyList<CashTransaction>> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams ctParams);
    Task<IReadOnlyList<CashTransaction>> GetByIBANAsync(string iban, CashTransactionParams ctParams);
}
