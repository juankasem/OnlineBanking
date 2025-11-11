using OnlineBanking.Application.Models.Address;
using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Models.Customer.Base;

public class BaseCustomerDto
{
    public string IdentificationNo { get; private set; }
    public IdentificationType IdentificationType { get; private set; }
    public string CustomerNo { get; set; }
    public int AppUserId { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Nationality { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public string TaxNumber { get; set; }
    public AddressDto Address { get; set; }

    public IEnumerable<BankAccountDto> BankAccounts { get; set; }
}
