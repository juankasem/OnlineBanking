using System;
using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Core.Domain.Validators;
public class CashTransactionValidator : AbstractValidator<CashTransaction>
{

    public string ReferenceNo { get; private set; }

    public CashTransactionType Type { get; private set; }

    public BankAssetType InitiatedBy { get; private set; }


    public string From { get; private set; }

    public string To { get; private set; }

    public decimal Amount { get; private set; }

    public int CurrencyId { get; private set; }

    public decimal Fees { get; set; }

    public string Description { get; set; }

    public decimal SenderAvailableBalance { get; set; }

    public decimal RecipientAvailableBalance { get; set; }

    public PaymentType PaymentType { get; set; }

    public DateTime TransactionDate { get; set; }

    public CashTransactionStatus Status { get; set; }
}