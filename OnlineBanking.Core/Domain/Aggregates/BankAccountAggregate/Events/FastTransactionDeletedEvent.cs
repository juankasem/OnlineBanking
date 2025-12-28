using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record FastTransactionDeletedEvent(Guid Id) : IDomainEvent;

