using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

public class Customer : BaseDomainEntity
{
    private readonly List<CustomerBankAccount> _customerBankAccounts = new List<CustomerBankAccount>();

    /// <summary>
    /// ID number
    /// </summary>
    public string IdentificationNo { get; private set; }

    // <summary>
    /// ID type (passport, ID, SSN, etc...)
    /// </summary>
    public IdentificationType IdentificationType { get; private set; }

    /// <summary>
    ///  Customer Number
    /// </summary>
    public string CustomerNo { get; private set; }

    /// <summary>
    /// App user Id
    /// </summary>
    public string AppUserId { get; set; }

    /// <summary>
    /// customer first name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// customer first name
    /// </summary>
    public string MiddleName { get; set; }

    /// <summary>
    /// customer last name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// customer Gender
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// customer birth Date
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// tax number
    /// </summary>
    public string TaxNumber { get; set; }

    /// <summary>
    /// customer address
    /// </summary>
    public Address Address { get; set; }
    
    //Many-to-many relationship
    public ICollection<CustomerBankAccount> CustomerBankAccounts { get { return _customerBankAccounts; } }

    private Customer(Guid id, string identificationNo, IdentificationType identificationType,
                        string customerNo, string appUserId,
                        string firstName, string middleName, string lastName,
                        string nationality, Gender gender, DateTime birthDate,
                        string taxNumber)
    {
        Id = id;
        IdentificationNo = identificationNo;
        IdentificationType = identificationType;
        CustomerNo = customerNo;
        AppUserId = appUserId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Nationality = nationality;
        Gender = gender;
        BirthDate = birthDate;
        TaxNumber = taxNumber;
    }

    public static Customer Create(string identificationNo, IdentificationType identificationType,
                                    string customerNo, string appUserId,
                                    string firstName, string middleName, string lastName,
                                    string nationality, Gender gender, DateTime birthDate,
                                    string taxNumber, Guid? id = null)
    {
        var validator = new CustomerValidator();

        var objectToValidate = new Customer(
            id ?? Guid.NewGuid(),
            identificationNo,
            identificationType,
            customerNo,
            appUserId,
            firstName,
            middleName,
            lastName,
            nationality,
            gender,
            birthDate,
            taxNumber);

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;
        var exception = new CustomerNotValidException("Customer is not valid");
        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));

        throw exception;
    }

    public void AddBankAccountToCustomer(CustomerBankAccount customerBankAccount) => _customerBankAccounts.Add(customerBankAccount);

    public void SetAddress(Address address) => Address = address;
}