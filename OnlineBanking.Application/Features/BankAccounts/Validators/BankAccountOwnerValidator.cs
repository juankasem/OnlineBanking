using FluentValidation;
using OnlineBanking.Application.Models.Customer;

namespace OnlineBanking.Application.Features.BankAccounts.Validators;

public class BankAccountOwnerValidator : AbstractValidator<AccountOwnerDto>
{
    public BankAccountOwnerValidator()
    {
        RuleFor(b => b.CustomerId)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(b => b.FullName)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required");
    }
}