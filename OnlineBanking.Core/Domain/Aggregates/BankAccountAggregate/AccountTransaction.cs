
namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

/// <summary>
/// Represents the relationship between a BankAccount and a CashTransaction.
/// This is a join entity for the many-to-many relationship.
/// </summary>
public class AccountTransaction
{
    #region Properties

    /// <summary>
    /// ID of the associated bank account
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Navigation property to the bank account
    /// </summary>
    public BankAccount Account { get; set; }

    /// <summary>
    /// ID of the associated cash transaction
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// Navigation property to the cash transaction
    /// </summary>
    public CashTransaction Transaction { get; set; }
    #endregion

    #region Constructor

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    public AccountTransaction()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AccountTransaction class
    /// </summary>
    /// <param name="accountId">ID of the bank account</param>
    /// <param name="transactionId">ID of the cash transaction</param>
    private AccountTransaction(Guid accountId, Guid transactionId)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));
        if (transactionId == Guid.Empty)
            throw new ArgumentException("Transaction ID cannot be empty.", nameof(transactionId));

        AccountId = accountId;
        TransactionId = transactionId;
    }
    
    #endregion

    #region Factory Method

    /// <summary>
    /// Creates a new AccountTransaction instance
    /// </summary>
    /// <param name="accountId">ID of the bank account</param>
    /// <param name="transactionId">ID of the cash transaction</param>
    /// <returns>A new AccountTransaction instance</returns>
    /// <exception cref="ArgumentException">Thrown when IDs are empty</exception>
    public static AccountTransaction Create(Guid accountId, Guid transactionId) => new(accountId, transactionId);

    #endregion
}