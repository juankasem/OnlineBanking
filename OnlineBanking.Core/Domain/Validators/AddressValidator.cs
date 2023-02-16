using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

namespace OnlineBanking.Core.Domain.Validators;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
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

        RuleFor(b => b.DistrictId)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .GreaterThan(0).WithMessage("Allowed Balance to use can't be less than zero");


        RuleFor(b => b.CityId)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .GreaterThan(0).WithMessage("Allowed Balance to use can't be less than zero");


        RuleFor(b => b.CountryId)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .GreaterThan(0).WithMessage("Allowed Balance to use can't be less than zero");

    }
}
