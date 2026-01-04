using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Transfer;

/// <summary>
/// Represents a request to transfer funds between two bank accounts.
/// Includes transaction details, sender/recipient IBANs, and participant information.
/// </summary>
public class MakeFundsTransferCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the base transaction details including amount, fees, and transaction metadata.
    /// </summary>
    public  BaseCashTransaction BaseCashTransaction { get; set; }

    /// <summary>
    /// Gets the IBAN of the sender's bank account.
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// Gets the IBAN of the recipient's bank account.
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// Gets the name of the sender.
    /// </summary>
    public string Sender { get; set; }

    /// <summary>
    /// Gets the name of the recipient.
    /// </summary>
    public string Recipient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MakeFundsTransferCommand"/> class.
    /// </summary>
    /// <param name="baseCashTransaction">The base transaction details</param>
    /// <param name="from">The sender's IBAN</param>
    /// <param name="to">The recipient's IBAN</param>
    /// <param name="sender">The sender's name</param>
    /// <param name="recipient">The recipient's name</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
    /// <exception cref="ArgumentException">Thrown when string parameters are empty or whitespace</exception>
    public MakeFundsTransferCommand(
        BaseCashTransaction baseCashTransaction,
        string from, 
        string to,
        string sender, 
        string recipient)
    {
        ArgumentNullException.ThrowIfNull(baseCashTransaction);
        ArgumentException.ThrowIfNullOrWhiteSpace(from);
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(sender);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);

        if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Sender and recipient IBANs cannot be the same.", nameof(to));

        BaseCashTransaction = baseCashTransaction;
        From = from;
        To = to;
        Sender = sender;
        Recipient = recipient;
    }
}