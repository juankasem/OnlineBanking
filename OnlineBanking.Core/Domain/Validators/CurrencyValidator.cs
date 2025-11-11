using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(c => c.Code)
            .NotNull().WithMessage("{propertyName} is required")
            .NotEmpty().WithMessage("{propertyName} can't be empty");

        RuleFor(c => c.Name)
        .NotNull().WithMessage("{propertyName} is required")
        .NotEmpty().WithMessage("{propertyName} can't be empty");

        RuleFor(c => c.Symbol)
        .NotNull().WithMessage("{propertyName} is required")
        .NotEmpty().WithMessage("{propertyName} can't be empty");
    }
}
