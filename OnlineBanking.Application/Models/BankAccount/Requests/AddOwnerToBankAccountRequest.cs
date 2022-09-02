using System;
using System.Collections.Generic;
using OnlineBanking.Application.Models.Customer;

namespace OnlineBanking.Application.Models.BankAccount.Requests;

public class AddOwnerToBankAccountRequest
{
    public Guid BankAccountId { get; set; }
    public List<AccountOwnerDto> AccountOwners { get; set; }
}