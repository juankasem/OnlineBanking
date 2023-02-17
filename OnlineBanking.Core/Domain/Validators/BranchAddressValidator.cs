using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class BranchAddressValidator : AbstractValidator<Address>
{
    public BranchAddressValidator()
    {
        RuleFor(b => b.Name)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.Street)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.ZipCode)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(b => b.District)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");


        RuleFor(b => b.City)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");


        RuleFor(b => b.Country)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}
