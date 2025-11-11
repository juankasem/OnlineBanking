using FluentValidation;
using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Features.BankAccounts.Validators;

public class AccountBanlanceValidator : AbstractValidator<AccountBalanceDto>
{
    public AccountBanlanceValidator()
    {
        RuleFor(b => b.Balance)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(b => b.AllowedBalanceToUse)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(b => b.MinimumAllowedBalance)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");
    }
}