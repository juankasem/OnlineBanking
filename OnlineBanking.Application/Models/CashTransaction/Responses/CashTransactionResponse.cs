namespace OnlineBanking.Application.Models.CashTransaction.Responses;

public class CashTransactionResponse
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string InitiatedBy { get; set; }
    public string From { get; set; }
    public string To { get; private set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public Money Amount { get; set; }
    public Money Fees { get; set; }
    public string Description { get; set; }
    public string PaymentType { get; set; }
    public string TransactionDate { get; set; }
    public string Status { get; set; }
    public Money AvailableBalance { get; set; }


    public CashTransactionResponse(string id, string type, string initiatedBy,
                                    string from, string to, string sender, string recipient, Money amount,
                                    Money fees, string description, string paymentType, string transactionDate,
                                    string status, Money availableBalance)
    {
        Id = id;
        Type = type;
        InitiatedBy = initiatedBy;
        From = from;
        To = to;
        Sender = sender;
        Recipient = recipient;
        Amount = amount;
        Fees = fees;
        Description = description;
        PaymentType = paymentType;
        TransactionDate = transactionDate;
        Status = status;
        AvailableBalance = availableBalance;
    }
}