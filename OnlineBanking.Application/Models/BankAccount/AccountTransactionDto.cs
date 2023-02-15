using System;
using OnlineBanking.Application.Models.CashTransaction;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.BankAccount;

public class AccountTransactionDto
{
    public CashTransactionType Type { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public Money Amount { get; private set; }
    public Money Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime TransactionDate { get; set; }
    public CashTransactionStatus Status { get; set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public string? From { get; private set; }
    public string? To { get; private set; }
    public AccountTransactionDto(CashTransactionType type, BankAssetType initiatedBy,
                                    Money amount, Money fees,
                                    string description, PaymentType paymentType,
                                    DateTime transactionDate, CashTransactionStatus status,
                                    string from = null, string to = null,
                                    string sender = null, string recipient = null)
    {
        Type = type;
        InitiatedBy = initiatedBy;
        Sender = sender;
        Recipient = recipient;
        From = from;
        To = to;
        Amount = amount;
        Fees = fees;
        Description = description;
        PaymentType = paymentType;
        TransactionDate = transactionDate;
        Status = status;
    }
}
