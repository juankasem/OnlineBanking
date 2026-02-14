using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Infrastructure.DTOs;
using System.Text.Json;

namespace OnlineBanking.Infrastructure.Services;

/// <summary>
/// Publishes domain events to Azure Service Bus.
/// Handles serialization, correlation, and metadata propagation for all event types.
/// </summary>
public interface IServiceBusPublisher
{
    /// <summary>
    /// Publishes a domain event to the configured Service Bus topic.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish</param>
    /// <exception cref="ArgumentNullException">Thrown when domainEvent is null</exception>
    Task PublishEventAsync(IDomainEvent domainEvent);
}

public class ServiceBusPublisher(
    ServiceBusClient serviceBusClient, 
    IOptions<ServiceBusOptions> options) 
    : IServiceBusPublisher
{
    private readonly ServiceBusClient serviceBusClient = serviceBusClient;
    private readonly string _topicName = options?.Value?.TransactionsTopic ?? 
        throw new ArgumentNullException(nameof(options));

    public async Task PublishEventAsync(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        // Convert to DTO for proper serialization
        var eventDto = DomainEventDto.FromDomainEvent(domainEvent);
        var messageBody = JsonSerializer.Serialize(eventDto);

        var message = new ServiceBusMessage(messageBody)
        {
            MessageId = Guid.NewGuid().ToString(),
            Subject = domainEvent.GetType().Name,
            CorrelationId = domainEvent.EventId.ToString(),
            ApplicationProperties =
            {
                { "EventType", domainEvent.EventType },
                { "EventId", domainEvent.EventId.ToString() },
                { "OccurredOn", domainEvent.OccurredOn.ToString("O") }            
            }
        };

        // Create a sender for the topic & Publish the message to it
        await using var sender = serviceBusClient.CreateSender(_topicName);
        await sender.SendMessageAsync(message);
    }
}
