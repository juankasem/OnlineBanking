using OnlineBanking.Application.Models.Currency;

namespace OnlineBanking.Application.Models.CashTransaction;

public class Money
{
    public decimal Value { get; set; }

    public CurrencyDto Currency { get; set; }

    public Money(decimal value, CurrencyDto currency)
    {
        Value = value;
        Currency = currency;
    }
}