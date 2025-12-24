
using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record BankAccountCreatedEvent(Guid Id,
    string AccountNo,
    string IBAN,
    BankAccountType Type,
    string BranchName,
    decimal Balance,
    string Currency) : IDomainEvent;
