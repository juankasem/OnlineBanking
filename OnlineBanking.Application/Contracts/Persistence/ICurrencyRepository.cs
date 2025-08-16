
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;
public interface ICurrencyRepository : IGenericRepository<Currency>
{
    Task<bool> ExistsAsync(int id);
}