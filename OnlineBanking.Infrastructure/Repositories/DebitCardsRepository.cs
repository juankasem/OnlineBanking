using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class DebitCardsRepository : GenericRepository<DebitCard>, IDebitCardsRepository
{
    public DebitCardsRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
}
