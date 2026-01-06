
namespace OnlineBanking.Application.Models.CashTransaction.Base;

public class BaseCashTransaction
{
    public string IBAN { get; set; }
    public CashTransactionType Type { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public MoneyDto Amount { get; set; }
    public MoneyDto Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime TransactionDate { get; set; }

    private BaseCashTransaction()
    {
    }

    public BaseCashTransaction(string iBAN, 
        CashTransactionType type, 
        BankAssetType initiatedBy,
        MoneyDto amount, 
        MoneyDto fees, 
        string description,
        PaymentType paymentType,
        DateTime transactionDate)
    {
        IBAN = iBAN;
        Type = type;
        InitiatedBy = initiatedBy;
        Amount = amount;
        Fees = fees ?? new MoneyDto(0, 1);
        Description = description;
        PaymentType = paymentType;
        TransactionDate = transactionDate == default ? DateTime.UtcNow : transactionDate;
    }
}

public class MoneyDto(decimal value, int currencyId)
{
    public decimal Value { get; set; } = value;
    public int CurrencyId { get; set; } = currencyId;
}