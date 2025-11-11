using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;

public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    public CountryRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {

    }
}
