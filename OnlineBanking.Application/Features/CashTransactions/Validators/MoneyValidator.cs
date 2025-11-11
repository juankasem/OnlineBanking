using FluentValidation;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MoneyValidator : AbstractValidator<Money>
{
    private readonly IUnitOfWork _uow;

    public MoneyValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(c => c.Value)
        .NotNull().WithMessage("{PropertyName} is required")
        .GreaterThan(0).WithMessage("{PropertyName} should be greater than {ComparisonValue}");

        RuleFor(b => b.Currency)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(c => c.Currency.Id)
        .GreaterThan(0)
        .MustAsync(async (id, token) =>
        {
            return !await _uow.Currencies.ExistsAsync(id);
        }).WithMessage("{PropertyName} does not exist");
    }
}
