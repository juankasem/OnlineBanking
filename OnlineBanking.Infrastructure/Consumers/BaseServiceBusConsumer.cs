
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineBanking.Infrastructure.Services;

namespace OnlineBanking.Infrastructure.Consumers;

public abstract class BaseServiceBusConsumer<TEvent>(
    ServiceBusClient client,
    ILogger logger,
    IOptions<ServiceBusOptions> options) : BackgroundService
    where TEvent : class
{
    private readonly ServiceBusClient _client = client;
    private readonly ILogger _logger = logger;
    private readonly string _topic = options.Value.TransactionsTopic;
    private readonly string _subscription = options.Value.SubscriptionName;

    override protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeMessageAsync(stoppingToken);
    }

    private async Task ConsumeMessageAsync(CancellationToken stoppingToken)
    {
        await using var processor = _client.CreateProcessor(_topic, _subscription,
           new ServiceBusProcessorOptions()
           {
               AutoCompleteMessages = false,
               MaxConcurrentCalls = 1,
               MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
           });

        processor.ProcessMessageAsync += ProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);
    }

    /// <summary>
    /// Handle a Service Bus message
    /// </summary>
    protected abstract Task ProcessMessageAsync(ProcessMessageEventArgs args);

    /// <summary>
    /// Handle Service Bus processing errors
    /// </summary>
    protected abstract Task ProcessErrorAsync(ProcessErrorEventArgs args);
}


