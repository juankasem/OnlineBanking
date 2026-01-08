using OnlineBanking.Application.Models.CashTransaction.Requests;

namespace OnlineBanking.Application.Features.CashTransactions.Validators;

public class CreateCashTransactionRequestValidator : AbstractValidator<CreateCashTransactionRequest>
{
    public CreateCashTransactionRequestValidator()
    {
        RuleFor(c => c.BaseCashTransaction)
            .SetValidator(new BaseCashTransactionValidator());
    }
}