using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class CashTransactionValidator : AbstractValidator<CashTransaction>
{
    public CashTransactionValidator()
    {
        RuleFor(c => c.Id)
         .NotNull().WithMessage("{PropertyName} is required")
         .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.ReferenceNo)
         .NotNull().WithMessage("Reference No is required")
         .NotEmpty().WithMessage("Reference No is required");

        RuleFor(c => c.Type)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.From)
       .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.To)
         .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.InitiatedBy)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.Amount)
          .GreaterThan(0).WithMessage("Amount should be greater than {ComparisonValue}");

        RuleFor(c => c.Fees)
          .GreaterThanOrEqualTo(0).WithMessage("Fees should be greater than {ComparisonValue}");

        RuleFor(c => c.PaymentType)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(c => c.TransactionDate)
        .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-60)).WithMessage("{PropertyName} must not be before {ComparisonValue}");
    }
}