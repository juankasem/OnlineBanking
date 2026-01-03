
using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class City : Entity<int>
{
    public string Name { get; set; }

    public int CountryId { get; set; }

    public Country Country { get; set; }
}
