using FluentValidation;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

public class MakeWithdrawalCommandValidator : AbstractValidator<MakeWithdrawalCommand>
{
    public MakeWithdrawalCommandValidator()
    {
        RuleFor(b => b.From)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}
