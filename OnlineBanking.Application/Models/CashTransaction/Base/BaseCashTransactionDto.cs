using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.CashTransaction.Base;

public class BaseCashTransactionDto
{
    public string IBAN { get; set; }
    public CashTransactionType Type { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public MoneyDto Amount { get; set; }
    public MoneyDto Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public string TransactionDate { get; set; }

    public class MoneyDto
    {
        public decimal Value { get; set; }
        public int CurrencyId { get; set; }
    }

    public BaseCashTransactionDto(string iBAN, CashTransactionType type, BankAssetType initiatedBy,
                                  MoneyDto amount, MoneyDto fees, string description, 
                                  PaymentType paymentType, string transactionDate)
    {
        IBAN = iBAN;
        Type = type;
        InitiatedBy = initiatedBy;
        Amount = amount;
        Fees = fees;
        Description = description;
        PaymentType = paymentType;
        TransactionDate = transactionDate;
    }
}