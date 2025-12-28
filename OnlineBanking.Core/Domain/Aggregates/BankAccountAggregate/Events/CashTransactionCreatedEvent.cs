using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public sealed record CashTransactionCreatedEvent(Guid Id,
    CashTransactionType Type,
    DateTime CashTransactionDate,
    string? SenderAccountIBAN,
    string? RecipientAccountIBAN,
    decimal Amount,
    string Currency,
    decimal Fees = 0) : IDomainEvent;
