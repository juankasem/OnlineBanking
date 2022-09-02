using System;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.CashTransaction.Responses;

public class CashTransactionResponse
{
    public string Id { get; set; }
    public CashTransactionType Type { get; set; }
    public BankAssetType InitiatedBy { get; set; }
    public string From { get; set; }
    public string To { get; private set; }
    public string Sender { get; set; }
    public string Recipient { get; set; }
    public Money Amount { get; set; }
    public Money Fees { get; set; }
    public string Description { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateTime TransactionDate { get; set; }
    public CashTransactionStatus Status { get; set; }
    public Money AvailableBalance { get; set; }


#nullable enable
    public string? CreditCardNo { get; set; }
    public string? DebitCardNo { get; set; }

    public CashTransactionResponse(string id, CashTransactionType type, BankAssetType initiatedBy,
                                    string from, string to, string sender, string recipient, Money amount,
                                    Money fees, string description, PaymentType paymentType, DateTime transactionDate,
                                    CashTransactionStatus status, Money availableBalance,
                                    string? creditCardNo = null, string? debitCardNo = null)
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
        CreditCardNo = creditCardNo;
        DebitCardNo = debitCardNo;
    }
}