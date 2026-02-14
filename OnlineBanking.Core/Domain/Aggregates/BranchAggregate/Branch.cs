using OnlineBanking.Core.Domain.Abstractions;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate.Events;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

public class Branch : AggregateRoot<int>
{
    private readonly List<BankAccount> _bankAccounts = [];

    public string Name { get; private set; }
    public Address Address { get; private set; }

    public IReadOnlyList<BankAccount> BankAccounts => _bankAccounts.AsReadOnly();

    private Branch(string name)
    {
        Name = name;
    }

    //factory methods
    public static Branch Create(string name)
    {
        var validator = new BranchValidator();
        var branch = new Branch(
            name
        );
        var validationResult = validator.Validate(branch);

        if (validationResult.IsValid)
        {
            // Add domain event
            branch.AddDomainEvent(
                new BranchCreatedEvent(branch.Id,
                branch.Name));

            return branch;
        }
        var exception = new BranchNotValidException("Branch is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }

    public void AddBankAccount(BankAccount bankAccount) => _bankAccounts.Add(bankAccount);
    public void SetName(string name) => Name = name;
    public void SetAddress(Address address) => Address = address;
}