
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;
using OnlineBanking.Infrastructure.Services;
using System.Text.Json;

namespace OnlineBanking.Infrastructure.Consumers;

public class CashTransactionCreatedEventConsumer(
        ServiceBusClient serviceBusClient,
        ILogger<CashTransactionCreatedEventConsumer> logger,
        IOptions<ServiceBusOptions> options) : 
        BaseServiceBusConsumer<CashTransactionCreatedEvent>(serviceBusClient, logger, options)
{
    protected override async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            logger.LogInformation("Received message: {messageBody}", body);

            var deserializedEvent = JsonSerializer.Deserialize<CashTransactionCreatedEvent>(body);

            if (deserializedEvent is not CashTransactionCreatedEvent cashTransactionCreatedEvent)
            {
                logger.LogError("Received null or invalid CashTransactionCreatedEvent");
                await args.AbandonMessageAsync(args.Message);
                return;
            }

            logger.LogInformation("Processing CashTransactionCreatedEvent: {@event}", cashTransactionCreatedEvent);

            // Simulate processing
            await Task.Delay(500, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing {Source}", args.EntityPath);
            try
            {
                await args.AbandonMessageAsync(args.Message);
            }
            catch (Exception abandonEx)
            {
                logger.LogError(abandonEx, "Failed to abandon message");
            }
        }
    }

    protected override Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing message: {errorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }
}
