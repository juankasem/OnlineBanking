using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Core.Domain.Validators
{
    public class FastTransactionValidator : AbstractValidator<FastTransaction>
    {
        
    }
}