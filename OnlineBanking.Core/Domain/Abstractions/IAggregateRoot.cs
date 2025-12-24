
namespace OnlineBanking.Core.Domain.Abstractions;

public interface IAggregateRoot<T> : IAggregateRoot, IEntity<T>
{
}

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
    void AddDomainEvent(IDomainEvent domainEvent);
}