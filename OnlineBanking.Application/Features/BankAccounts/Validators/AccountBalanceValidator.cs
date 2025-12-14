

namespace OnlineBanking.Application.Features.BankAccounts.Validators;

public class AccountBalanceValidator : AbstractValidator<AccountBalanceDto>
{
    public AccountBalanceValidator()
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