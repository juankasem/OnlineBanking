using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using OnlineBanking.Application.Features.Addresses.Validators;
using OnlineBanking.Application.Features.Branch.Commands;

namespace OnlineBanking.Application.Features.Branch.Validators;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {

        RuleFor(c => c.Name)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(c => c.Address).SetValidator(new BaseAddressValidator());

    }
}