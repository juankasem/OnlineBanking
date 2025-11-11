using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class UtilityPaymentRepository : GenericRepository<UtilityPayment>, IUtilityPaymentRepository
{
    public UtilityPaymentRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
}
