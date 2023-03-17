using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Models.CashTransaction.Base;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class BaseCashTransactionValidator : AbstractValidator<BaseCashTransactionDto>
{
    private readonly IUnitOfWork _uow;

    public BaseCashTransactionValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(c => c.Type)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.InitiatedBy)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.Amount).SetValidator(new MoneyValidator(_uow));
        
        RuleFor(c => c.Fees).SetValidator(new MoneyValidator(_uow));

        RuleFor(c => c.PaymentType)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(c => c.TransactionDate)
        .GreaterThanOrEqualTo(DateTime.Now).WithMessage("{PropertyName} must not be before {ComparisonValue}");
    }
}