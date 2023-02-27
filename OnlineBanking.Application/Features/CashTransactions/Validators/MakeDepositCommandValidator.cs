using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CashTransactions.Commands;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MakeDepositCommandValidator : AbstractValidator<MakeDepositCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeDepositCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(b => b.BaseCashTransaction).SetValidator(new BaseCashTransactionValidator(_uow));

        RuleFor(b => b.To)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");
    }
}
