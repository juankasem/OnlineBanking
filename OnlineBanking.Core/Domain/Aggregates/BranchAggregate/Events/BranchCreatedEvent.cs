using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate.Events;

public record BranchCreatedEvent(int BranchId,
    string BranchName) : DomainEvent;

