
namespace OnlineBanking.Application.Features.CashTransactions.Create;

/// <summary>
/// Represents a request to create a cash transaction.
/// Contains all transaction information including amount, parties, and transaction type.
/// Note: ReferenceNo and Status are managed by the domain/handler, not set in the command.
/// </summary>
public class CreateCashTransactionCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the type of cash transaction (Deposit, Withdrawal, Transfer).
    /// </summary>
    public CashTransactionType CashTransactionType { get; set; }

    /// <summary>
    /// Gets the bank asset that initiated the transaction (ATM, BankAccount, POS, etc.).
    /// </summary>
    public BankAssetType InitiatedBy { get; set; }

    /// <summary>
    /// Gets the IBAN of the transaction originator (sender/from account).
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// Gets the IBAN of the transaction recipient (to account).
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// Gets the transaction amount.
    /// Must be greater than zero.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets the currency ID for the transaction.
    /// Must be a valid currency identifier.
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// Gets the transaction fees.
    /// Can be zero for some transaction types.
    /// </summary>
    public decimal Fees { get; set; }

    /// <summary>
    /// Gets the transaction description.
    /// Provides context or memo for the transaction.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets the payment type (Salary, Rent, Utility, etc.).
    /// Categorizes the nature of the transaction.
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Gets the date when the transaction should be processed.
    /// </summary>
    public DateTime TransactionDate { get; set; }
}
