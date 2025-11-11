using FluentValidation;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MakeDepositCommandValidator : AbstractValidator<MakeDepositCommand>
{
    public MakeDepositCommandValidator()
    {
        RuleFor(b => b.To)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}