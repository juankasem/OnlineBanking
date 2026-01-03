using System.Text.Json.Serialization;

namespace OnlineBanking.Core.Domain.Abstractions;

/// <summary>
/// Base abstract record for all domain events.
/// Provides common implementation of IDomainEvent properties to avoid repetition across event records.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// Generated once at creation time.
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the event occurred in UTC.
    /// </summary>
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Fully qualified type name of the event for serialization/deserialization.
    /// </summary>
    public string EventType => GetType().AssemblyQualifiedName ?? string.Empty;
}
