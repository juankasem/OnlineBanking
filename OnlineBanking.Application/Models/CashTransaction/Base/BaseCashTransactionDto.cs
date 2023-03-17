using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.CashTransaction.Base;

public class BaseCashTransactionDto
{
    public string IBAN { get; set; }
    public string ReferenceNo { get; set; }
    public CashTransactionType Type { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public Money Amount { get; set; }
    public Money Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime TransactionDate { get; set; }
    public CashTransactionStatus Status { get; set; }


    public BaseCashTransactionDto(string iBAN, string referenceNo, CashTransactionType type, BankAssetType initiatedBy,
                                Money amount, Money fees, string description, PaymentType paymentType,
                                DateTime transactionDate, CashTransactionStatus status)
    {
        IBAN = iBAN;
        ReferenceNo = referenceNo;
        Type = type;
        InitiatedBy = initiatedBy;
        Amount = amount;
        Fees = fees;
        Description = description;
        PaymentType = paymentType;
        TransactionDate = transactionDate;
        Status = status;
    }
}