using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CreditCardsRepository : GenericRepository<CreditCard>, ICreditCardsRepository
{
    public CreditCardsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<CreditCard>> GetCustomerCreditCardsAsync(string customerNo)
    {
        IQueryable<CreditCard> query = _dbContext.CreditCards.AsQueryable();

        return await query.Where(c => c.CustomerNo == customerNo)
                        .Include(c => c.BankAccount)
                        .AsNoTracking()
                        .ToListAsync();
    }
}
