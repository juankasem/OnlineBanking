using System.Collections.Generic;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

public class Branch : BaseDomainEntity
{
    private readonly List<BankAccount> _bankAccounts = new List<BankAccount>();

    public new int Id { get; set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }

    public ICollection<BankAccount> BankAccounts { get { return _bankAccounts; } }

    private Branch(string name)
    {
        Name = name;
    }

    //factory methods
    public static Branch Create(string name)
    {
        var validator = new BranchValidator();

        var objectToValidate = new Branch(
            name
        );

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;

        var exception = new BranchNotValidException("Branch is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }

    public void AddBankAccount(BankAccount bankAccount) => _bankAccounts.Add(bankAccount);
   
    public void SetAddress(Address address) => Address = address;
}