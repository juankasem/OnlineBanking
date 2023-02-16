
using FluentValidation;
using OnlineBanking.Application.Features.FastTransactions.Commands;

namespace OnlineBanking.Application.Features.FastTransactions.Validators
{
    public class CreateFastTransactionCommandValidator : AbstractValidator<CreateFastTransactionCommand>
    {
        
        public CreateFastTransactionCommandValidator()
        {
            
        }
    }
}