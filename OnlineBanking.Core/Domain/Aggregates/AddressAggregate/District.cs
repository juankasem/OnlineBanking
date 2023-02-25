using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class District : BaseDomainEntity
{
    public new int Id { get; set; }

    public string Name { get; set; }

    public int CityId { get; set; }

    public City City { get; set; }
}
