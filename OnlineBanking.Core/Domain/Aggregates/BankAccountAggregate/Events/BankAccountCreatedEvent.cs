
using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;

public record BankAccountCreatedEvent(Guid Id,
    string AccountNo,
    string IBAN,
    BankAccountType Type,
    int BranchId,
    decimal Balance,
    int CurrencyId) : IDomainEvent;
