using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record FastTransactionCreatedEvent(Guid Id, 
    Guid BankAccountId,
    string RecipientIBAN,
    string RecipientName, 
    decimal Amount) : DomainEvent;

