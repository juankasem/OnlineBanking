using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Infrastructure.Persistence;

namespace OnlineBanking.Infrastructure.Repositories;
public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    public CountryRepository(OnlineBankDbContext dbContext) : base(dbContext)
    {

    }
}
