
namespace OnlineBanking.Application.Contracts.Persistence;

public interface ICashTransactionsRepository : IGenericRepository<CashTransaction>
{
    Task<(IReadOnlyList<CashTransaction>, int)> GetByAccountNoOrIBANAsync(string accountNoOrIBAN, CashTransactionParams cashTransactionParams);
    Task<(IReadOnlyList<CashTransaction>, int)> GetByIBANAsync(string iban, CashTransactionParams cashTransactionParams);
}