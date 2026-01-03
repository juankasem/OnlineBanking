using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class BaseCashTransactionValidator : AbstractValidator<BaseCashTransactionDto>
{
    private readonly IUnitOfWork _uow;

    public BaseCashTransactionValidator(IUnitOfWork uow)
    {
        ArgumentNullException.ThrowIfNull(uow);
        _uow = uow;

        RuleFor(c => c.IBAN)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.Type)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.InitiatedBy)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.Amount)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .SetValidator(new MoneyDtoValidator());

        RuleFor(c => c.PaymentType)
        .NotNull()
        .WithMessage("{PropertyName} is required");

        RuleFor(c => c.TransactionDate)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} cannot be empty")
        .Must(IsValidDateFormat)
        .WithMessage("{PropertyName} must be in format dd/MM/yyyy");
    }

    /// <summary>
    /// Validates that the transaction date string can be parsed as a valid date.
    /// </summary>
    private static bool IsValidDateFormat(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return false;

        return DateTime.TryParseExact(
            dateString,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}

public class MoneyDtoValidator : AbstractValidator<MoneyDto>
{
    public MoneyDtoValidator()
    {
        RuleFor(c => c.Value)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .GreaterThan(0)
        .WithMessage("{PropertyName} should be greater than {ComparisonValue}");
    }
}
