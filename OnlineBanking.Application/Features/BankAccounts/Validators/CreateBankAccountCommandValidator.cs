using FluentValidation;
using OnlineBanking.Application.Features.BankAccounts.Commands;

namespace OnlineBanking.Application.Features.BankAccounts.Validators;

public class CreateBankAccountCommandValidator : AbstractValidator<CreateBankAccountCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateBankAccountCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(b => b.AccountNo)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .MinimumLength(16).WithMessage("Minimum number of characters of {b.AccountNo } should be 16 characters");

        RuleFor(b => b.IBAN)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .MinimumLength(20).WithMessage("Minimum number of characters of {PropertyName} should be {ComparisonValue} characters");

        RuleFor(b => b.Type)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(b => b.BranchId)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(b => b.Balance)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(b => b.AllowedBalanceToUse)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(b => b.MinimumAllowedBalance)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must not be before {ComparisonValue}");

        RuleFor(c => c.CurrencyId)
        .GreaterThan(0)
        .MustAsync(async (id, token) =>
        {
            return await _uow.Currencies.ExistsAsync(id);
        }).WithMessage("{PropertyName} does not exist");
    }
}