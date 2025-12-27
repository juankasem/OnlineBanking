
using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class Country : BaseDomainEntity<int>
{
    public string Name { get; set; }
}