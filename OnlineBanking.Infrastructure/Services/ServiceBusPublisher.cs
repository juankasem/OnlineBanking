
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace OnlineBanking.Infrastructure.Services;

public interface IServiceBusPublisher
{
    Task PublishEventAsync<T>(T eventToPublish);
}

public class ServiceBusPublisher(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> options) : IServiceBusPublisher
{
    private readonly ServiceBusClient serviceBusClient = serviceBusClient;
    private readonly string _topicName = options?.Value?.TransactionsTopic ?? throw new ArgumentNullException(nameof(options));

    public async Task PublishEventAsync<T>(T eventToPublish)
    {
       var messageBody = JsonSerializer.Serialize((object)eventToPublish);

       var message = new ServiceBusMessage(messageBody)
       {
         MessageId = Guid.NewGuid().ToString(),
         Subject = typeof(T).Name,
         ApplicationProperties =
         {
             { "EventType", typeof(T).FullName ?? string.Empty }
         }
       };

        // Create a sender for the topic
        await using var topicSender = serviceBusClient.CreateSender(_topicName);
        await topicSender.SendMessageAsync(message);
    }
}
