using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineBanking.Application.Features.CashTransactions.Events;
using System.Text.Json;

namespace OnlineBanking.Infrastructure.Services;

public class BankAccountService(
        ServiceBusClient serviceBusClient,
        ILogger<BankAccountService> logger,
        IOptions<ServiceBusOptions> options) : BackgroundService
{
    private readonly ServiceBusClient serviceBusClient = serviceBusClient;
    private readonly ILogger<BankAccountService> logger = logger;
    private readonly string _topicName = options?.Value?.TransactionsTopic ?? "transactions";
    private readonly string _subscriptionName = options?.Value?.SubscriptionName ?? "transactionsnotifications";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         await ConsumeMessageAsync(stoppingToken);
    }

    private async Task ConsumeMessageAsync(CancellationToken cancellationToken)
    {
        await using var processor = serviceBusClient.CreateProcessor(_topicName, _subscriptionName,
            new ServiceBusProcessorOptions()
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        processor.ProcessMessageAsync += args => ProcessMessageAsync(args, "topic");
        processor.ProcessErrorAsync += ProcessErrorAsync;
        
        await processor.StartProcessingAsync(cancellationToken);

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // expected on shutdown
        }
        finally
        {
            await processor.StopProcessingAsync();
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args, string source)
    {
        try
        {
            var body = args.Message.Body.ToString();
            logger.LogInformation("Received message: {messageBody}", body);

            var cashTransactionCreatedEvent = JsonSerializer.Deserialize<CashTransactionCreatedEvent>(body);

            if (cashTransactionCreatedEvent is null)
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
            logger.LogError(ex, "Error processing {Source}", source);
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

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error processing message: {errorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }
}
