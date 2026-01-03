using OnlineBanking.Core.Domain.Abstractions;

namespace OnlineBanking.Infrastructure.DTOs;

/// <summary>
/// Data transfer object for domain events.
/// Flattens both base and derived event properties for serialization.
/// </summary>
public class DomainEventDto
{
    public Guid EventId { get; set; }
    public DateTimeOffset OccurredOn { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Dictionary<string, object?> Data { get; set; } = new();

    public static DomainEventDto FromDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var dto = new DomainEventDto
        {
            EventId = domainEvent.EventId,
            OccurredOn = domainEvent.OccurredOn,
            EventType = domainEvent.EventType
        };

        // Reflection to extract all public properties except base event properties
        var eventType = domainEvent.GetType();
        var properties = eventType.GetProperties(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Skip base DomainEvent properties
            if (property.Name is nameof(EventId) or nameof(OccurredOn) or nameof(EventType))
                continue;

            var value = property.GetValue(domainEvent);
            dto.Data[property.Name] = value;
        }

        return dto;
    }
}
