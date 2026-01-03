using OnlineBanking.Application.Features.CashTransactions.Validators;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Transfer;

public class MakeFundsTransferCommandValidator : AbstractValidator<MakeFundsTransferCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeFundsTransferCommandValidator(IUnitOfWork uow)
    {
        ArgumentNullException.ThrowIfNull(uow);
        _uow = uow;

        // Validate base transaction
        RuleFor(c => c.BaseCashTransaction)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .SetValidator(new BaseCashTransactionValidator(uow));

        RuleFor(b => b.From)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.To)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.BaseCashTransaction.Fees)
            .SetValidator(new MoneyDtoValidator());
    }
}
