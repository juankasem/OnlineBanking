
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;
public class CityRepository : GenericRepository<City>, ICityRepository
{
    public CityRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {

    }
}
