using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record BankAccountActivatedEvent(string IBAN, 
    BankAccountType Type) : IDomainEvent
{
    public Guid EventId => Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? string.Empty;
}

