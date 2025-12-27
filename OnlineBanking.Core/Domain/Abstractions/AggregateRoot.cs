using OnlineBanking.Core.Domain.Common;

namespace OnlineBanking.Core.Domain.Abstractions;

public abstract class AggregateRoot<TId> : BaseDomainEntity<TId>, IAggregateRoot<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }
}

