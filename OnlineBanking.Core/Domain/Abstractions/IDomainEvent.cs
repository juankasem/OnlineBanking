using MediatR;

namespace OnlineBanking.Core.Domain.Abstractions;

public interface IDomainEvent : INotification
{
   public Guid EventId => Guid.NewGuid();
   public DateTimeOffset OccurredOn => DateTimeOffset.UtcNow;
   public string EventType => GetType().AssemblyQualifiedName;
}

