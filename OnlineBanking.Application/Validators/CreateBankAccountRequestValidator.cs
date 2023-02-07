using FluentValidation;
using OnlineBanking.Application.Models.BankAccount.Requests;

namespace OnlineBanking.Application.Features.BankAccount.Validators;

public class CreateBankAccountRequestValidator : AbstractValidator<CreateBankAccountRequest>
{
    public CreateBankAccountRequestValidator()
    {
        
    }
}
