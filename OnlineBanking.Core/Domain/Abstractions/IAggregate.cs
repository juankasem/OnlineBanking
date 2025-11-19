

namespace OnlineBanking.Core.Domain.Abstractions;

public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
    void AddDomainEvent(IDomainEvent domainEvent);
}