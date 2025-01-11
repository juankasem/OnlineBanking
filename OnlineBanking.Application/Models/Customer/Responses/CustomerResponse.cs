using OnlineBanking.Application.Models.Address;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Models.Customer.Responses;

public class CustomerResponse
{
    public string IdentificationNo { get; private set; }
    public IdentificationType IdentificationType { get; private set; }
    public string CustomerNo { get; private set; }
    public string AppUserId { get; private set; }
    public string FirstName { get; private set; }
    public string MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string Nationality { get; private set; }
    public Gender Gender { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string TaxNumber { get; private set; }
    public AddressDto Address { get; private set; }

    public List<BankAccountDto> BankAccounts { get; set; }
}