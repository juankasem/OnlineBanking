using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Models.CashTransaction.Requests;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class CreateCashTransactionRequestValidator : AbstractValidator<CreateCashTransactionRequest>
{
    private readonly IUnitOfWork _uow;
    public CreateCashTransactionRequestValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(c => c.From)
       .NotNull().WithMessage("{PropertyName} is required")
       .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.BaseCashTransaction).SetValidator(new BaseCashTransactionValidator(_uow));
    }
}