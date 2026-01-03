
using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class Country : Entity<int>
{
    public string Name { get; set; }
}