using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class Country : BaseDomainEntity
{
    public new int Id { get; set; }

    public string Name { get; set; }
}