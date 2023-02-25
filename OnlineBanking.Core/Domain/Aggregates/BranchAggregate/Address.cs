
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.BranchAggregate;

public class Address
{
    public string Name { get; private set; }

    public string Street { get; private set; }

    public string ZipCode { get; private set; }

    public string District { get; set; }

    public string City { get; set; }

    public string Country { get; private set; }

    private Address(string name, string street, string zipCode, string district, string city, string country)
    {
        Name = name;
        Street = street;
        ZipCode = zipCode;
        District = district;
        City = city;
        Country = country;
    }

    public static Address Create(string name, string street, string zipCode, string district, string city, string country)
    {
          var validator = new BranchAddressValidator();

        var objectToValidate = new Address(
            name, street, zipCode, district, city, country);

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;


       var exception = new BranchAddressNotValidException("Address is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;    
    }
}
