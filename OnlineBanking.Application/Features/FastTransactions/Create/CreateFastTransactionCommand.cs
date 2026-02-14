
namespace OnlineBanking.Application.Features.FastTransactions.Create;

/// <summary>
/// Represents a request to create a fast transaction between two bank accounts.
/// Includes transaction details and participant information.
/// </summary>
public class CreateFastTransactionCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the IBAN of the sender's bank account.
    /// </summary>
    public string IBAN { get; set; }

    /// <summary>
    /// Gets the IBAN of the recipient's bank account.
    /// </summary>
    public string RecipientIBAN { get; set; }

    /// <summary>
    /// Gets the name of the recipient.
    /// </summary>
    public string RecipientName { get; set; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    public decimal Amount { get; set; }
}
