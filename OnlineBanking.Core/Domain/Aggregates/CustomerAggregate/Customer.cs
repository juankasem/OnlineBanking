using System;
using System.Collections.Generic;
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
    public string IDNo { get; private set; }

    // <summary>
    /// ID type (passport, ID, SSN)
    /// </summary>
    public DocumentType IDType { get; private set; }

    /// <summary>
    ///  Customer Number
    /// </summary>
    public string CustomerNo { get; set; }

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

    private Customer(Guid id, string idNo, DocumentType idType,
                        string customerNo, string appUserId,
                        string firstName, string middleName, string lastName,
                        string nationality, Gender gender, DateTime birthDate,
                        string taxNumber, Address address)
    {
        Id = id;
        IDNo = idNo;
        CustomerNo = customerNo;
        AppUserId = appUserId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Nationality = nationality;
        Gender = gender;
        BirthDate = birthDate;
        TaxNumber = taxNumber;
        Address = address;
    }

    public static Customer Create(string idNo, DocumentType idType,
                                    string customerNo, string appUserId,
                                    string firstName, string middleName, string lastName,
                                    string nationality, Gender gender, DateTime birthDate,
                                    string taxNumber, Address address, Guid? id = null)
    {
        var validator = new CustomerValidator();

        var objectToValidate = new Customer(
            id ?? Guid.NewGuid(),
            idNo,
            idType,
            customerNo,
            appUserId,
            firstName,
            middleName,
            lastName,
            nationality,
            gender,
            birthDate,
            taxNumber,
            address);

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;
        var exception = new CustomerNotValidException("Customer is not valid");
        validationResult.Errors.ForEach(vr => exception.ValidationErrors.Add(vr.ErrorMessage));

        throw exception;
    }

    public void AddBankAccountToCustomer(CustomerBankAccount customerBankAccount)
    {
        _customerBankAccounts.Add(customerBankAccount);
    }
}