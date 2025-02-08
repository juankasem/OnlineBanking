using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CashTransactions.Commands;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MakeWithdrawalCommandValidator : AbstractValidator<MakeWithdrawalCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeWithdrawalCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(b => b.BaseCashTransaction).SetValidator(new BaseCashTransactionValidator(_uow));

        RuleFor(b => b.From)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}
