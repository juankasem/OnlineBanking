using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record BankAccountDeactivatedEvent(string IBAN,
    BankAccountType Type) : IDomainEvent;
