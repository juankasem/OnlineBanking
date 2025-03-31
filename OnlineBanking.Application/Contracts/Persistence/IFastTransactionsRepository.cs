
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;

public interface IFastTransactionsRepository : IGenericRepository<FastTransaction>
{
    Task<(IReadOnlyList<FastTransaction>, int)> GetByIBANAsync(string iban, FastTransactionParams fastTransactionParams);
}
