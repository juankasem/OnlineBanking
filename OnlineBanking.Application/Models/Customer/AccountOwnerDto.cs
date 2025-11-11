namespace OnlineBanking.Application.Models.Customer;

public class AccountOwnerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerNo { get; set; }
    public string FullName { get; set; }

    public AccountOwnerDto(Guid customerId, string customerNo, string fullName)
    {
        CustomerId = customerId;
        CustomerNo = customerNo;
        FullName = fullName;
    }
}
