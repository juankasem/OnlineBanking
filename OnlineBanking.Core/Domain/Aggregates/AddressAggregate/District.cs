using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class District : Entity<Guid>
{
    public new int Id { get; set; }

    public string Name { get; set; }

    public int CityId { get; set; }

    public City City { get; set; }
}
