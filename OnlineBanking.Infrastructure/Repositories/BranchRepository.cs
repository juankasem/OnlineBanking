using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {
    }
}
