using System;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class FastTransaction : BaseDomainEntity
{
    public string RecipientIBAN { get; set; }
    public string RecipientName { get; set; }
    public decimal Amount { get; set; }
    public Guid BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; }

    private FastTransaction(Guid id, string recipientIBAN, string recipientName,
                            decimal amount, Guid bankAccountId)
    {
        Id = id;
        RecipientIBAN = recipientIBAN;
        RecipientName = recipientName;
        Amount = amount;
        BankAccountId = bankAccountId;
    }

        public static FastTransaction Create(string recipientIBAN, string recipientName,
                                             decimal amount, Guid bankAccountId, Guid? id = null)
    {
        var validator = new FastTransactionValidator();
        
        var objectToValidate = new FastTransaction(
        id ?? Guid.NewGuid(),
        recipientIBAN,
         recipientName, 
         amount, 
         bankAccountId
        );

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new FastTransactionNotValidException("Fast Transaction is not valid");
        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));
        throw exception;
    }
}
