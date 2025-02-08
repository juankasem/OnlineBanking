using FluentValidation;
using OnlineBanking.Application.Models.Branch;

namespace OnlineBanking.Application.Features.Branch.Validators;

public class BranchAddressValidator : AbstractValidator<BranchAddressDto>
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
