
namespace OnlineBanking.Infrastructure.Services;

public class ServiceBusOptions
{
    public string TransactionsTopic { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}

