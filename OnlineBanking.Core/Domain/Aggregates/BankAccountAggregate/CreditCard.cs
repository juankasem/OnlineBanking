using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

public class CreditCard : BaseDomainEntity
{
    public string CreditCardNo { get; set; }
    public string CustomerNo { get; set; }
    public DateTime ValidTo { get; set; }
    public int SecurityCode { get; set; }
    public Guid BankAccountId { get; set; }
    public BankAccount BankAccount { get; set; }
    public bool IsActive { get; set; }
    public string? PIN { get; set; }

    private CreditCard(Guid id, string creditCardNo, string customerNo, DateTime validTo,
                        int securityCode, Guid bankAccountId, string pIN = null, bool isActive = false)
    {
        Id = id;
        CreditCardNo = 	creditCardNo;
        PIN = pIN;
        CustomerNo = customerNo;
        ValidTo = validTo;
        SecurityCode = securityCode;
        BankAccountId = bankAccountId;
    }

    public static CreditCard Create(string creditCardNo, string customerNo, DateTime validTo,
                        int securityCode, Guid bankAccountId, bool isActive = false, Guid? id = null, string? pIN = null)
    {
        var validator = new CreditCardValidator();

        var objectToValidate = new CreditCard(
            id ?? Guid.NewGuid(),
            creditCardNo,
            customerNo, 
            validTo,
            securityCode,
            bankAccountId,
            pIN,
            isActive
        );
        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new CreditCardNotValidException("Credit Card is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }
}
