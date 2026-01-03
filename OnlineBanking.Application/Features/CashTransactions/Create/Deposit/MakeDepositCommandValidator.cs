
using OnlineBanking.Application.Features.CashTransactions.Validators;

namespace OnlineBanking.Application.Features.CashTransactions.Create.Deposit;

public class MakeDepositCommandValidator : AbstractValidator<MakeDepositCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeDepositCommandValidator(IUnitOfWork uow)
    {
        ArgumentNullException.ThrowIfNull(uow);
        _uow = uow;

        // Validate base transaction
        RuleFor(c => c.BaseCashTransaction)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .SetValidator(new BaseCashTransactionValidator(uow));

        RuleFor(b => b.To)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} can't be empty");
    }
}