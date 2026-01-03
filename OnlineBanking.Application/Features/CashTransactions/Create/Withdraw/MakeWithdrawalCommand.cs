using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

/// <summary>
/// Represents a request to withdraw funds from a specified bank account.
/// Includes transaction details and the IBAN of the account initiating the withdrawal.
/// </summary>
public class MakeWithdrawalCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the base transaction details including amount, fees, and transaction metadata.
    /// </summary>
    public required BaseCashTransactionDto BaseCashTransaction { get; set; }

    /// <summary>
    /// Gets the IBAN of the account withdrawing funds.
    /// </summary>
    public required string From { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeWithdrawalCommand"/> class.
    /// </summary>
    /// <param name="baseCashTransaction">The base transaction details</param>
    /// <param name="from">The IBAN of the account withdrawing funds</param>
    /// <exception cref="ArgumentNullException">Thrown when baseCashTransaction is null</exception>
    /// <exception cref="ArgumentException">Thrown when from is empty or whitespace</exception>
    public MakeWithdrawalCommand(BaseCashTransactionDto baseCashTransaction, string from)
    {
        ArgumentNullException.ThrowIfNull(baseCashTransaction);
        ArgumentException.ThrowIfNullOrWhiteSpace(from);

        BaseCashTransaction = baseCashTransaction;
        From = from;
    }
}