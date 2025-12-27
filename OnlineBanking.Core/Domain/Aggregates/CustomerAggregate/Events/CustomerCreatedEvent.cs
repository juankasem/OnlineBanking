using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Aggregates.CustomerAggregate.Events;

public record CustomerCreatedEvent(Guid Id,
    string CustomerNo,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Nationality) : IDomainEvent;

