using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Core.Domain.Validators;
public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(b => b.Type)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.BranchId)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.CurrencyId)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance can't be less than zero");

        RuleFor(b => b.AllowedBalanceToUse)
        .GreaterThanOrEqualTo(0).WithMessage("Allowed Balance to use can't be less than zero");

        RuleFor(b => b.Debt)
            .GreaterThanOrEqualTo(0).WithMessage("Debt can't be less than zero");
    }
}
