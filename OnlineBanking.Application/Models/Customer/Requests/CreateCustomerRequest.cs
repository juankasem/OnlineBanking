
using OnlineBanking.Application.Models.Address;

namespace OnlineBanking.Application.Models.Customer.Requests;

public class CreateCustomerRequest
{
    public string IdentificationNo { get; set; }

    public IdentificationType IdentificationType { get; set; }

    public string CustomerNo { get; set; }

    public string AppUserId { get; set; }

    public string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string LastName { get; set; }

    public string Nationality { get; set; }

    public Gender Gender { get; set; }

    public string BirthDate { get; set; }

    public string TaxNumber { get; set; }

    public AddressDto Address { get; set; }
}