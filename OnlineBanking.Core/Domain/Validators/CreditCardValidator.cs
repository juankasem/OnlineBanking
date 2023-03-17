using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class CreditCardValidator : AbstractValidator<CreditCard>
{
    public CreditCardValidator()
    {
        RuleFor(c => c.CreditCardNo)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.ValidTo)
            .GreaterThanOrEqualTo(DateTime.Now).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(c => c.SecurityCode)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.CustomerNo)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.BankAccountId)
        .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}
