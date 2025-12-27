
namespace OnlineBanking.Application.Features.BankAccounts.Create;

public class CreateBankAccountCommandValidator : AbstractValidator<CreateBankAccountCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateBankAccountCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

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