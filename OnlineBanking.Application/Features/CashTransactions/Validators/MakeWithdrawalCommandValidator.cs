using FluentValidation;
using OnlineBanking.Application.Features.CashTransactions.Commands;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MakeWithdrawalCommandValidator : AbstractValidator<MakeWithdrawalCommand>
{
    public MakeWithdrawalCommandValidator()
    {
        RuleFor(b => b.From)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}
