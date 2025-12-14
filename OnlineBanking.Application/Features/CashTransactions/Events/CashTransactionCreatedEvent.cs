namespace OnlineBanking.Application.Features.CashTransactions.Events;

public class CashTransactionCreatedEvent
{
    public Guid CashTransactionId { get; set; }
}
