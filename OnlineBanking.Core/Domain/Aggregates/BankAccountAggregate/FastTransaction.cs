
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

/// <summary>
/// Represents a fast transaction between bank accounts.
/// Fast transactions are expedited fund transfers with minimal processing time.
/// </summary>
public class FastTransaction : BaseDomainEntity
{
    #region Properties

    /// <summary>
    /// IBAN of the recipient's bank account
    /// </summary>
    public string RecipientIBAN { get; private set; }

    /// <summary>
    /// Full name of the transaction recipient
    /// </summary>
    public string RecipientName { get; private set; }

    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// ID of the sender's bank account
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// Navigation property to the sender's bank account
    /// </summary>
    public BankAccount BankAccount { get; set; }

    #endregion

    private FastTransaction(Guid id, Guid bankAccountId,
                            string recipientIBAN, string recipientName,
                            decimal amount)
    {
        Id = id;
        BankAccountId = bankAccountId;
        RecipientIBAN = recipientIBAN;
        RecipientName = recipientName;
        Amount = amount;
    }

    #region Factory Method

    /// <summary>
    /// Creates a new FastTransaction with validation.
    /// </summary>
    /// <param name="bankAccountId">ID of the sender's bank account</param>
    /// <param name="recipientIBAN">IBAN of the recipient</param>
    /// <param name="recipientName">Name of the recipient</param>
    /// <param name="amount">Transaction amount</param>
    /// <param name="id">Optional transaction ID; generates new GUID if not provided</param>
    /// <returns>A validated FastTransaction instance</returns>
    /// <exception cref="FastTransactionNotValidException">Thrown when validation fails</exception>
    public static FastTransaction Create(Guid bankAccountId, string recipientIBAN, string recipientName,
                                         decimal amount, Guid? id = null)
    {
        var validator = new FastTransactionValidator();

        var objectToValidate = new FastTransaction(
        id ?? Guid.NewGuid(),
        bankAccountId,
        recipientIBAN,
        recipientName,
        amount
        );

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new FastTransactionNotValidException("Fast Transaction is not valid");
        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));
        throw exception;
    }

    #endregion
}
