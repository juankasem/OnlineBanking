using OnlineBanking.Application.Models.CashTransaction.Base;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.CashTransaction.Requests;

public class CreateCashTransactionRequest 
{
    public BaseCashTransactionDto BaseCashTransaction { get; set; }

    #nullable enable
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Sender { get; set; }
    public string? Recipient { get; set; }
    public string? CreditCardNo { get; set; }
    public string? DebitCardNo { get; set; }
}

public class MoneyDto
{
    public decimal Value { get; set; }
    public decimal CurrencyId { get; set; }
}