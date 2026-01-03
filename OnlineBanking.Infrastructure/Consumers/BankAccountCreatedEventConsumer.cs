using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.Events;
using OnlineBanking.Infrastructure.Services;
using System.Text.Json;

namespace OnlineBanking.Infrastructure.Consumers;

public class BankAccountCreatedEventConsumer(
        ServiceBusClient serviceBusClient,
        ILogger<BankAccountCreatedEventConsumer> logger,
        IOptions<ServiceBusOptions> options) :
        BaseServiceBusConsumer<BankAccountCreatedEvent>(serviceBusClient, logger, options)
{
    protected override async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            logger.LogInformation("Received message: {messageBody}", body);

            var deserializedEvent = JsonSerializer.Deserialize<BankAccountCreatedEvent>(body);

            if (deserializedEvent is not BankAccountCreatedEvent bankAccountCreatedEvent)
            {
                logger.LogError("Received null or invalid BankAccountCreatedEvent");
                await args.AbandonMessageAsync(args.Message);
                return;
            }

            logger.LogInformation("Processing BankAccountCreatedEvent: {@event}", bankAccountCreatedEvent);

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


