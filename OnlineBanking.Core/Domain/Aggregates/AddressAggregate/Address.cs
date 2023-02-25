using OnlineBanking.Core.Domain.Common;
using OnlineBanking.Core.Domain.Entities;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

namespace OnlineBanking.Core.Domain.Aggregates.AddressAggregate;

public class Address : BaseDomainEntity
{
    public string Name { get; private set; }

    public string Street { get; private set; }

    public string ZipCode { get; private set; }

    public int DistrictId { get; private set; }

     public District District { get; set; }

    public int CityId { get; private set; }
    
    public City City { get; set; }
    
    public int CountryId { get; private set; }

    public Country Country { get; set; }

    public string AppUserId { get; set; }

    public AppUser AppUser { get; set; }

    private Address(string name, string street, string zipCode,
                    int districtId, int cityId, int countryId, 
                    string appUserId, bool isDeleted = false)
    {
        Name = name;
        Street = street;
        ZipCode = zipCode;
        DistrictId = districtId;
        CityId = cityId;
        CountryId = CountryId;
        AppUserId = appUserId;
        IsDeleted = isDeleted;
    }

    public static Address Create(string name, string street, string zipCode,
                    int districtId, int cityId, int countryId, 
                    string appUserId, bool isDeleted = false)
    {
        var validator = new AddressValidator();

        var objectToValidate = new Address(
            name, street, zipCode, districtId, cityId, countryId, 
            appUserId,
            isDeleted
        );

        var validationResult = validator.Validate(objectToValidate);

        if (validationResult.IsValid) return objectToValidate;


       var exception = new AddressNotValidException("Address is not valid");
        validationResult.Errors.ForEach(er => exception.ValidationErrors.Add(er.ErrorMessage));
        throw exception;
    }
}