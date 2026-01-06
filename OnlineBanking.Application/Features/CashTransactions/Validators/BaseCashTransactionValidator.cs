using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

/// <summary>
/// Validator for base cash transaction data.
/// Validates IBAN, transaction type, asset type, amount, payment type, and transaction date.
/// Ensures transaction date is not in the past and not too far in the future (max 30 days).
/// </summary>
public class BaseCashTransactionValidator : AbstractValidator<BaseCashTransaction>
{
    private const string DateFormat = "dd/MM/yyyy";
    private const int MaxFutureDays = 30;
    private readonly IUnitOfWork _uow;

    public BaseCashTransactionValidator(IUnitOfWork uow)
    {
        ArgumentNullException.ThrowIfNull(uow);
        _uow = uow;

        ConfigureIbanValidation();
        ConfigureTransactionTypeValidation();
        ConfigureAssetTypeValidation();
        ConfigureAmountValidation();
        ConfigurePaymentTypeValidation();
        ConfigureTransactionDateValidation();
    }

    /// <summary>
    /// Configures validation rules for IBAN field.
    /// </summary>
    private void ConfigureIbanValidation()
    {
        RuleFor(c => c.IBAN)
            .NotNull()  
            .WithMessage("{PropertyName} is required")
            .NotEmpty()     
            .WithMessage("{PropertyName} cannot be empty");
    }

    /// <summary>
    /// Configures validation rules for transaction type.
    /// </summary>  
    private void ConfigureTransactionTypeValidation()
    {
        RuleFor(c => c.Type)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .IsInEnum()
            .WithMessage("{PropertyName} must be a valid transaction type");
    }

    /// <summary>
    /// Configures validation rules for initiated by (bank asset type).
    /// </summary>
    private void ConfigureAssetTypeValidation()
    {
        RuleFor(c => c.InitiatedBy)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .IsInEnum()
            .WithMessage("{PropertyName} must be a valid bank asset type");
    }

    /// <summary>
    /// Configures validation rules for transaction amount.
    /// </summary>
    private void ConfigureAmountValidation()
    {
        RuleFor(c => c.Amount)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .SetValidator(new MoneyDtoValidator());
    }

    /// <summary>
    /// Configures validation rules for payment type.
    /// </summary>
    private void ConfigurePaymentTypeValidation()
    {
        RuleFor(c => c.PaymentType)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .IsInEnum()
            .WithMessage("{PropertyName} must be a valid payment type");
    }

    /// <summary>
    /// Configures validation rules for transaction date.
    /// Validates that date is not in the past and not more than 30 days in the future.
    /// </summary>
    private void ConfigureTransactionDateValidation()
    {
        RuleFor(c => c.TransactionDate)
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .Must(IsNotOldDate)
            .WithMessage("{PropertyName} cannot be in the past. Please use today's date or a future date.")
            .Must(IsNotTooFarInFuture)
            .WithMessage($"{{PropertyName}} cannot be more than {MaxFutureDays} days in the future.");
    }

    /// <summary>
    /// Validates that the transaction date is not in the past.
    /// Accepts today's date and any future date.
    /// </summary>
    private static bool IsNotOldDate(DateTime transactionDate)
    {
        var today = DateTime.UtcNow.Date;
        return transactionDate.Date >= today;
    }

    /// <summary>
    /// Validates that the transaction date is not more than 30 days in the future.
    /// </summary>
    private static bool IsNotTooFarInFuture(DateTime transactionDate)
    {
        var maxFutureDate = DateTime.UtcNow.Date.AddDays(MaxFutureDays);
        return transactionDate.Date <= maxFutureDate;
    }
}

/// <summary>
/// Validator for money data transfer objects.
/// Validates that amount is positive and currency ID is valid.
/// </summary>
public class MoneyDtoValidator : AbstractValidator<MoneyDto>
{
    /// <summary>
    /// Initializes a new instance of MoneyDtoValidator.
    /// </summary>
    /// <param name="uow">The unit of work for database operations. Optional for basic validation.</param>
    public MoneyDtoValidator()
    {
        ConfigureAmountValidation();
        ConfigureCurrencyIdValidation();
    }

    /// <summary>
    /// Configures validation rules for monetary amount.
    /// Ensures amount is positive and has valid decimal places.
    /// </summary>
    private void ConfigureAmountValidation()
    {
        RuleFor(c => c.Value)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .GreaterThan(0)
            .WithMessage("Amount value must be greater than zero")
            .LessThanOrEqualTo(decimal.MaxValue)
            .WithMessage("Amount value  exceeds maximum allowed value")
            .PrecisionScale(18, 4, ignoreTrailingZeros: true)
            .WithMessage("Amount value must have at most 4 decimal places");
    }

    /// <summary>
    /// Configures validation rules for currency ID.
    /// Ensures currency ID is positive.
    /// </summary>
    private void ConfigureCurrencyIdValidation()
    {
        RuleFor(c => c.CurrencyId)
            .NotNull()
            .WithMessage("Currency is required")
            .GreaterThan(0)
            .WithMessage("Currency must be a valid currency identifier");

    }
}
    