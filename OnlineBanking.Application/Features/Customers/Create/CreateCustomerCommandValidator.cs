
namespace OnlineBanking.Application.Features.Customers.Create;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{

    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.IdentificationNo)
          .NotNull().WithMessage("{PropertyName} is required")
          .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.IdentificationType)
          .NotNull().WithMessage("{PropertyName} is required")
          .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.CustomerNo)
          .NotNull().WithMessage("{PropertyName} is required")
          .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.FirstName)
          .NotNull().WithMessage("{PropertyName} is required")
          .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.LastName)
          .NotNull().WithMessage("{PropertyName} is required")
          .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.Nationality)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");

        RuleFor(c => c.TaxNumber)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} can't be empty");
    }
}

