using MediatR;

namespace OnlineBanking.Core.Domain.Abstractions;

public interface IDomainEvent : INotification
{
   public Guid EventId { get; }
   public DateTimeOffset OccurredOn { get; }
    public string EventType { get; }
}

