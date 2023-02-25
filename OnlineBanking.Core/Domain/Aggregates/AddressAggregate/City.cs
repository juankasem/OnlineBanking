using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class City : BaseDomainEntity
{
    public new int Id { get; set; }

    public string Name { get; set; }

    public int CountryId { get; set; }

    public Country Country { get; set; }
}
