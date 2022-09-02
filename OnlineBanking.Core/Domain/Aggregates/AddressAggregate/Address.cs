using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate
{
    public class Address : BaseDomainEntity
    {
        public string Name { get; private set; }

        public string Street { get; private set; }

        public string ZipCode { get; private set; }

        public int DistrictId { get; private set; }

        public int CityId { get; private set; }

        public int CountryId { get; private set; }

    }
}