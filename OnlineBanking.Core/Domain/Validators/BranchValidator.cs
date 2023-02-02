using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class BranchValidator : AbstractValidator<Branch>
{
    public BranchValidator()
    {
        RuleFor(b => b.Name)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");

    }
}