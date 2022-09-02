using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

namespace OnlineBanking.Application.Contracts.Persistence;
public interface ICountryRepository : IGenericRepository<Country>
{

}