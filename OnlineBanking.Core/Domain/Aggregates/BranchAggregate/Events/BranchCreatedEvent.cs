using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate.Events;

public class BranchCreatedEvent(int BranchId,
    string BranchName) : IDomainEvent;

