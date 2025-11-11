using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
{
    public CurrencyRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> ExistsAsync(int currencyId)
    {
        return await _dbContext.Currencies.AnyAsync(c => c.Id == currencyId);
    }
}