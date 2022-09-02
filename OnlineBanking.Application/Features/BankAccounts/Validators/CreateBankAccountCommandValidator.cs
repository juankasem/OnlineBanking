using FluentValidation;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.BankAccounts.Commands;

namespace OnlineBanking.Application.Features.BankAccounts.Validators;

public class CreateBankAccountCommandValidator : AbstractValidator<CreateBankAccountCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateBankAccountCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(b => b.AccountNo)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .Length(8).WithMessage("{PropertyName} should be {ComparisonValue} characters");

        RuleFor(b => b.IBAN)
        .NotNull().WithMessage("{PropertyName} is required")
        .NotEmpty().WithMessage("{PropertyName} is required")
        .Length(18).WithMessage("{PropertyName} should be {ComparisonValue} characters");

        RuleFor(b => b.Type)
        .NotNull().WithMessage("{PropertyName} is required");        

        RuleFor(b => b.BranchId)
        .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(b => b.AccountBalance).SetValidator(new AccountBanlanceValidator());
    
        RuleFor(c => c.CurrencyId)
        .GreaterThan(0)
        .MustAsync(async (id, token) =>
        {
            return !await _uow.Currencies.ExistsAsync(id);
        }).WithMessage("{PropertyName} does not exist");

        RuleForEach(b => b.AccountOwners)
        .SetValidator(c => new BankAccountOwnerValidator());
    }
}