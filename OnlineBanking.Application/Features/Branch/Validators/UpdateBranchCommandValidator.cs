using FluentValidation;
using OnlineBanking.Application.Features.Addresses.Validators;
using OnlineBanking.Application.Features.Branch.Commands;

namespace OnlineBanking.Application.Features.Branch.Validators;


public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(c => c.BranchId)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .GreaterThan(0);

        RuleFor(c => c.Name)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.Address).SetValidator(new BaseAddressValidator());
    }
}
