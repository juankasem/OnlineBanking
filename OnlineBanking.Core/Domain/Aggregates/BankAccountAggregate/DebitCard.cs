using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class DebitCard : Entity<Guid>
{
    #region Properties

    /// <summary>
    /// Credit card number
    /// </summary>
    public string DebitCardNo { get; private set; }

    /// <summary>
    /// Customer number associated with the card
    /// </summary>
    public string CustomerNo { get; private set; }

    /// <summary>
    /// Card expiration date
    /// </summary>
    public DateTime ValidTo { get; private set; } = DateTime.Now;

    /// <summary>
    /// Card security code (CVV)
    /// </summary>
    public int SecurityCode { get; private set; }

    /// <summary>
    /// ID of the associated bank account
    /// </summary>
    public Guid BankAccountId { get; private set; }

    /// <summary>
    /// Navigation property to the bank account
    /// </summary>
    public BankAccount? BankAccount { get; private set; } // Made nullable to satisfy CS8618

    /// <summary>
    /// Card activation status
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Card PIN (nullable, can be empty string initially)
    /// </summary>
    public string? PIN { get; private set; }

    #endregion

    #region Constructor

    private DebitCard(Guid id, string debitCardNo, string customerNo, DateTime validTo,
                        int securityCode, Guid bankAccountId, string? pIN = null, bool isActive = false)
    {
        Id = id;
        DebitCardNo = debitCardNo;
        PIN = pIN;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
        BankAccountId = bankAccountId;
        IsActive = isActive;
    }

    #endregion

    #region Factory Method

    /// <summary>
    /// Creates a new CreditCard instance with validation.
    /// </summary>
    /// <param name="creditCardNo">Credit card number</param>
    /// <param name="customerNo">Customer number</param>
    /// <param name="validTo">Card expiration date</param>
    /// <param name="securityCode">Card security code (CVV)</param>
    /// <param name="bankAccountId">ID of the associated bank account</param>
    /// <param name="id">Optional card ID; generates new GUID if not provided</param>
    /// <param name="pin">Optional card PIN</param>
    /// <returns>A validated CreditCard instance</returns>
    /// <exception cref="CreditCardNotValidException">Thrown when validation fails</exception>
    public static DebitCard Create(
        string creditCardNo, 
        string customerNo, 
        DateTime validTo,
        int securityCode, 
        Guid bankAccountId, 
        Guid? id = null, 
        string? pIN = null)
    {
        var validator = new DebitCardValidator();

        var debitCard = new DebitCard(
            id ?? Guid.NewGuid(),
            creditCardNo,
            customerNo,
            validTo,
            securityCode,
            bankAccountId,
            pIN
        );
        var validationResult = validator.Validate(debitCard);

        if (validationResult.IsValid) 
            return debitCard;

        var exception = new CreditCardNotValidException("Credit Card is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Activates the credit card
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// Deactivates the credit card
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Sets the PIN for the credit card
    /// </summary>
    /// <param name="pin">PIN value</param>
    public void SetPIN(string pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        PIN = pin;
    }
    #endregion
}