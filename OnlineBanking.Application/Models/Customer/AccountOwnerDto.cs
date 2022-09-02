using System;

namespace OnlineBanking.Application.Models.Customer;

public class AccountOwnerDto
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; }

    public AccountOwnerDto(Guid customerId, string fullName)
    {
        CustomerId = customerId;
        FullName = fullName;
    }
}
