using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CashTransactions.Commands;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class MakeFundsTransferCommandValidator : AbstractValidator<MakeFundsTransferCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeFundsTransferCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(b => b.BaseCashTransaction).SetValidator(new BaseCashTransactionValidator(_uow));

        RuleFor(b => b.From)
                    .NotNull().WithMessage("{PropertyName} is required")
                    .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(b => b.To)
                    .NotNull().WithMessage("{PropertyName} is required")
                    .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.BaseCashTransaction.Fees).SetValidator(new MoneyDtoValidator());
    }
}
