using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

/// <summary>
/// Represents a request to create a deposit cash transaction to a specified bank account.
/// </summary>
public class MakeDepositCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the base transaction details including amount, IBAN, fees, and transaction metadata.
    /// </summary>
    public BaseCashTransaction BaseCashTransaction { get; set; }

    /// <summary>
    /// Gets the IBAN of the account receiving the deposit.
    /// </summary>
    public string To { get; set; }

    public MakeDepositCommand(BaseCashTransaction baseCashTransaction, string to)
    {
        ArgumentNullException.ThrowIfNull(baseCashTransaction);
        ArgumentException.ThrowIfNullOrWhiteSpace(to);

        BaseCashTransaction = baseCashTransaction;
        To = to;
    }
}
