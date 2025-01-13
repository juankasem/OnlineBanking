
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICashTransactionsRepository : IGenericRepository<CashTransaction>
{
    Task<(IReadOnlyList<CashTransaction>, int)> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams ctParams);
    Task<(IReadOnlyList<CashTransaction>, int)> GetByIBANAsync(string iban, CashTransactionParams ctParams);
}