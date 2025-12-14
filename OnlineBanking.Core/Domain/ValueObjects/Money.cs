

namespace OnlineBanking.Core.Domain.ValueObjects;

public sealed record Money(decimal Amount, int CurrencyId)
{
    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (CurrencyId != other.CurrencyId) throw new InvalidOperationException("Currency mismatch");
        return this with { Amount = Decimal.Round(Amount + other.Amount, 2) };
    }

    public Money Subtract(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (CurrencyId != other.CurrencyId) throw new InvalidOperationException("Currency mismatch");
        return this with { Amount = Decimal.Round(Amount - other.Amount, 2) };
    }

    public bool IsNegative() => Amount < 0m;
}