
namespace OnlineBanking.Application.Features.CashTransactions.Create.Withdraw;

public class MakeWithdrawalCommandValidator : AbstractValidator<MakeWithdrawalCommand>
{
    private readonly IUnitOfWork _uow;

    public MakeWithdrawalCommandValidator(IUnitOfWork uow)
    {
        ArgumentNullException.ThrowIfNull(uow);
        _uow = uow;

        RuleFor(b => b.From)
        .NotNull()
        .WithMessage("{PropertyName} is required")
        .NotEmpty()
        .WithMessage("{PropertyName} can't be empty");
    }
}
