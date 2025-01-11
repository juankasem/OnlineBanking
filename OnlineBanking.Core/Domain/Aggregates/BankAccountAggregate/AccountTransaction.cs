namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class AccountTransaction
{
    public Guid AccountId { get; set; }
    public BankAccount Account { get; set; }

    public Guid TransactionId { get; set; }
    public CashTransaction Transaction { get; set; }

    private AccountTransaction(Guid accountId, Guid transactionId)
    {
        AccountId = accountId;
        TransactionId = transactionId;
    }

    public static AccountTransaction Create(Guid accountId, Guid transactionId) => new(accountId, transactionId);
}