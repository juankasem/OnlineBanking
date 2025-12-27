using OnlineBanking.Application.Features.Branch.Validators;

namespace OnlineBanking.Application.Features.Branch.Create;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(c => c.Name)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.Address).SetValidator(new BranchAddressValidator());
    }
}