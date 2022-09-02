using System;
using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Validators;
public class CashTransactionValidator : AbstractValidator<CashTransaction>
{
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

    /// <summary>
    /// fees of transaction
    /// </summary>
    public decimal Fees { get; set; }

    /// <summary>
    /// given description of transaction
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// sender's bank account available balance
    /// </summary>
    public decimal SenderAvailableBalance { get; set; }

    /// <summary>
    /// Recipient's bank account available balance
    /// </summary>
    public decimal RecipientAvailableBalance { get; set; }

    /// <summary>
    /// payment type like rent, salary, etc...
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// date of transaction
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// transaction status
    /// </summary>
    public CashTransactionStatus Status { get; set; }
}