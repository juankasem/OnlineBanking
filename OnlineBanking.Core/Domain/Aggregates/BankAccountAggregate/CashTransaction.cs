using System;
using System.Collections.Generic;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class CashTransaction : BaseDomainEntity
{
    private readonly List<AccountTransaction> _accountTransactions = new List<AccountTransaction>();

    /// <summary>
    /// unique reference number
    /// </summary>
    public string ReferenceNo { get; private set; }

    /// <summary>
    /// transaction type
    /// </summary>
    public CashTransactionType Type { get; private set; }

    /// <summary>
    /// bank asset such as bank account, POS, ATM
    /// </summary>
    public BankAssetType InitiatedBy { get; private set; }

    /// <summary>
    /// IBAN of bank account of the sender
    /// </summary>
    public string From { get; private set; }

    /// <summary>
    /// IBAN of bank account of the recipient
    /// </summary>
    public string To { get; private set; }

    /// <summary>
    /// amount of credited funds
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// currency of credited funds
    /// </summary>
    public int CurrencyId { get; private set; }

    public Currency Currency { get; private set; }

    /// <summary>
    /// fees of transaction
    /// </summary>
    public decimal Fees { get; private set; }

    /// <summary>
    /// given description of transaction
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// sender's bank account available balance
    /// </summary>
    public decimal SenderAvailableBalance { get; private set; }

    /// <summary>
    /// Recipient's bank account available balance
    /// </summary>
    public decimal RecipientAvailableBalance { get; private set; }

    /// <summary>
    /// payment type like rent, salary, etc...
    /// </summary>
    public PaymentType PaymentType { get; private set; }

    /// <summary>
    /// date of transaction
    /// </summary>
    public DateTime TransactionDate { get; private set; }

    /// <summary>
    /// transaction status
    /// </summary>
    public CashTransactionStatus Status { get; private set; } = CashTransactionStatus.Pending;

#nullable enable //Nullable props

    /// <summary>
    /// sender
    /// </summary>
    public string? Sender { get; private set; }

    /// <summary>
    /// sender
    /// </summary>
    public string? Recipient { get; private set; }

    /// <summary>
    /// credit card number
    /// </summary>
    public string? CreditCardNo { get; private set; }

    /// <summary>
    /// debit card number
    /// </summary>
    public string? DebitCardNo { get; private set; }

    public IReadOnlyList<AccountTransaction> AccountTransactions { get { return _accountTransactions; } }

    private CashTransaction(Guid id, string referenceNo, CashTransactionType type, BankAssetType initiatedBy,
                            string from, string to, decimal amount, int currencyId, decimal fees, string description,
                            decimal senderAvailableBalance, decimal recipientAvailableBalance,
                            PaymentType paymentType, DateTime transactionDate,
                             string? sender = null, string? recipient = null,
                            string? creditCardNo = null, string? debitCardNo = null)
    {
        Id = id;
        ReferenceNo = referenceNo;
        Type = type;
        InitiatedBy = initiatedBy;
        From = from;
        To = to;
        Sender = sender;
        Recipient = recipient;
        Amount = amount;
        CurrencyId = currencyId;
        Fees = fees;
        Description = description;
        SenderAvailableBalance = senderAvailableBalance;
        RecipientAvailableBalance = recipientAvailableBalance;
        PaymentType = paymentType;
        TransactionDate = transactionDate;
        CreditCardNo = creditCardNo;
        DebitCardNo = debitCardNo;
    }

    //Factory Method
    public static CashTransaction Create(string referenceNo, CashTransactionType type, BankAssetType initiatedBy,
                                        string from, string to, decimal amount, int currencyId,
                                        decimal fees, string description, decimal senderAvailableBalance, decimal recipientAvailableBalance,
                                        PaymentType paymentType, DateTime transactionDate, string? sender = null, string? recipient = null,
                                        string? creditCardNo = null, string? debitCardNo = null, Guid? id = null)
    {
        var validator = new CashTransactionValidator();
        var objectToValidate = new CashTransaction(
        id ?? Guid.NewGuid(),
        referenceNo ?? $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}",
        type,
        initiatedBy,
        from,
        to,
        amount,
        currencyId,
        fees,
        description,
        senderAvailableBalance,
        recipientAvailableBalance,
        paymentType,
        transactionDate,
        sender,
        recipient,
        creditCardNo,
        debitCardNo
        );

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new CashTransactionNotValidException("Transaction is not valid");
        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));
        throw exception;
    }

    public void UpdateStatus(CashTransactionStatus status)
    {
        Status = status;
        LastModifiedOn = DateTime.UtcNow;
    }
}