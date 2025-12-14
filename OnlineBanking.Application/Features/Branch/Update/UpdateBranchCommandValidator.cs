
using OnlineBanking.Application.Features.Branch.Validators;

namespace OnlineBanking.Application.Features.Branch.Update;

public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(c => c.BranchId)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .GreaterThan(0);

        RuleFor(c => c.BranchName)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.BranchAddress).SetValidator(new BranchAddressValidator());
    }
}
